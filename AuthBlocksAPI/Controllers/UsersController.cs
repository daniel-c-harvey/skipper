using System.Linq.Expressions;
using API.Shared.Controllers;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksModels.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthBlocksModels.SystemDefinitions;
using NetBlocks.Models;
using Models.Shared.Common;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseModelController<ApplicationUser, UserModel>
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) : base(userService)
    {
        _userService = userService;
        
        // Add custom sort expressions
        AddSortExpression(nameof(UserModel.UserName), e => e.UserName ?? string.Empty);
        AddSortExpression(nameof(UserModel.Email), e => e.Email ?? string.Empty);
        AddSortExpression(nameof(UserModel.NormalizedUserName), e => e.NormalizedUserName ?? string.Empty);
        AddSortExpression(nameof(UserModel.NormalizedEmail), e => e.NormalizedEmail ?? string.Empty);
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
        return await base.Get(query);
    }

    // Override base Get by ID to provide proper authorization
    [HttpGet("{id:long}")]
    public override async Task<ActionResult<ApiResultDto<UserModel>>> Get(long id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId != id && !User.IsInRole(SystemRole.UserAdmin))
        {
            return Forbid();
        }
        return await base.Get(id);
    }

    // Simplified: Update user by id with UpdateUserRequest
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResultDto<UserInfo>>> UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId != id && !User.IsInRole(SystemRoleConstants.UserAdmin))
        {
            return Forbid();
        }
        var user = await _userService.GetActiveUserByIdAsync(id);
        if (user == null)
        {
            var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found").Fail("User does not exist");
            return NotFound(new ApiResultDto<UserInfo>(resultFailure));
        }
        // Update user properties
        user.UserName = request.UserName ?? user.UserName;
        user.Email = request.Email ?? user.Email;
        user.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _userService.UpdateUserAsync(user);
        if (!updateResult.Succeeded)
        {
            var resultFailure = ApiResult<UserInfo>.CreateFailResult("Update failed");
            foreach (var error in updateResult.Errors)
            {
                resultFailure.Fail(error.Description);
            }
            return BadRequest(new ApiResultDto<UserInfo>(resultFailure));
        }
        var roles = await _userService.GetActiveUserRolesAsync(user);
        var userInfo = new UserInfo
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
        var resultSuccess = ApiResult<UserInfo>.CreatePassResult(userInfo).Inform("User updated successfully");
        return Ok(new ApiResultDto<UserInfo>(resultSuccess));
    }

    // Override base Delete to add authorization and self-deletion prevention
    [HttpDelete("{id:long}")]
    [HierarchicalRoleAuthorize(Roles = SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto>> Delete(long id)
    {
        // Prevent admin from deleting themselves
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id)
        {
            var resultFailure = ApiResult.CreateFailResult("Cannot delete your own account")
                .Fail("Self-deletion not allowed");
            return BadRequest(new ApiResultDto(resultFailure));
        }
        return await base.Delete(id);
    }

    // Add role management endpoints
    [HttpPost("{id}/roles")]
    [Authorize(Roles = SystemRoleConstants.UserAdmin)]
    public async Task<ActionResult<ApiResultDto>> AddUserToRole(long id, [FromBody] UserRoleRequest request)
    {
        try
        {
            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            await _userService.AddUserToRoleAsync(user, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User added to role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            var resultError = ApiResult.CreateFailResult("An error occurred while adding user to role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    [HttpDelete("{id}/roles")]
    [Authorize(Roles = SystemRoleConstants.UserAdmin)]
    public async Task<ActionResult<ApiResultDto>> RemoveUserFromRole(long id, [FromBody] UserRoleRequest request)
    {
        try
        {
            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            await _userService.RemoveUserFromRoleAsync(user, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User removed from role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            var resultError = ApiResult.CreateFailResult("An error occurred while removing user from role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    // Client-specific endpoint: Get user as UserInfo (with roles)
    [HttpGet("info/{id:long}")]
    public async Task<ActionResult<ApiResultDto<UserInfo>>> GetUserInfo(long id)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId != id && !User.IsInRole("Admin"))
        {
            return Forbid();
        }
        var user = await _userService.GetActiveUserByIdAsync(id);
        if (user == null)
        {
            var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found").Fail("User does not exist");
            return NotFound(new ApiResultDto<UserInfo>(resultFailure));
        }
        var roles = await _userService.GetActiveUserRolesAsync(user);
        var userInfo = new UserInfo
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
        var resultSuccess = ApiResult<UserInfo>.CreatePassResult(userInfo).Inform("User retrieved successfully");
        return Ok(new ApiResultDto<UserInfo>(resultSuccess));
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
} 