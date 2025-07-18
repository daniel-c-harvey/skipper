using System.Linq.Expressions;
using API.Shared.Controllers;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthBlocksModels.Converters;
using AuthBlocksModels.SystemDefinitions;
using NetBlocks.Models;
using Models.Shared.Common;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseModelController<ApplicationUser, UserModel, IUserService>
{
    private readonly IHierarchicalRoleService _authRoleService;

    public UsersController(IUserService userService, IHierarchicalRoleService authRoleService) : base(userService)
    {
        _authRoleService = authRoleService;
        
        // Add custom sort expressions
        AddSortExpression(nameof(UserModel.UserName), e => e.UserName ?? string.Empty);
        AddSortExpression(nameof(UserModel.Email), e => e.Email ?? string.Empty);
        AddSortExpression(nameof(UserModel.EmailConfirmed), e => e.EmailConfirmed);
        AddSortExpression(nameof(UserModel.PhoneNumber), e => e.PhoneNumber ?? string.Empty);
        AddSortExpression(nameof(UserModel.PhoneNumberConfirmed), e => e.PhoneNumberConfirmed);
        AddSortExpression(nameof(UserModel.TwoFactorEnabled), e => e.TwoFactorEnabled);
        AddSortExpression(nameof(UserModel.LockoutEnabled), e => e.LockoutEnabled);
        AddSortExpression(nameof(UserModel.AccessFailedCount), e => e.AccessFailedCount);
    }
    
    [HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto<PagedResult<UserModel>>>> Get(PagedQuery query)
    {
        var currentUserId = GetCurrentUserId();
        return await base.Get(query, (predicate, parameters) => Manager.GetPage(currentUserId, predicate, parameters));
    }

    // Override base Get by ID to provide proper authorization
    [HierarchicalRoleAuthorize]
    public override async Task<ActionResult<ApiResultDto<UserModel>>> Get(long id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId != id && !User.IsInRole(SystemRole.UserAdmin))
        {
            return Forbid();
        }
        return await base.Get(id);
    }
    
    [HierarchicalRoleAuthorize]
    public override async Task<ActionResult<ApiResultDto<UserModel>>> Post(UserModel model)
    {
        // Add logic to allow users to update their own basic info
        var currentUserId = GetCurrentUserId();
        if (currentUserId != model.Id && !(await _authRoleService.HasRoleOrInheritsAsync(GetCurrentUserRoles(), SystemRoleConstants.UserAdmin)))
        {
            return Forbid();
        }
        return await base.Post(model);
    }

    
    // Override base Delete to add authorization and self-deletion prevention
    [HttpDelete("{id:long}")]
    [HierarchicalRoleAuthorize(Roles = SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto>> Delete(long id)
    {
        // Prevent user from deleting themselves
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id)
        {
            var resultFailure = ApiResult.CreateFailResult("Cannot delete your own account")
                .Fail("Self-deletion not allowed");
            return BadRequest(new ApiResultDto(resultFailure));
        }
        return await base.Delete(id);
    }

    protected override Expression<Func<ApplicationUser, bool>> BuildSearchPredicate(string? search)
    {
        if (string.IsNullOrEmpty(search))
            return e => true;

        return e => e.UserName!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.Email!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.NormalizedUserName!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.NormalizedEmail!.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdClaim?.Value, out var userId) ? userId : 0;
    }

    private IList<string> GetCurrentUserRoles()
    {
        // Get user's roles from JWT claims
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }
} 