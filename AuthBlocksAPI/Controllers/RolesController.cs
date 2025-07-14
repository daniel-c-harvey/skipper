using System.Linq.Expressions;
using API.Shared.Controllers;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using AuthBlocksModels.SystemDefinitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetBlocks.Models;
using Models.Shared.Common;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : BaseModelController<ApplicationRole, RoleModel, IRoleService>
{
    public RolesController(IRoleService roleService) : base(roleService)
    {
        // Add custom sort expressions
        AddSortExpression(nameof(RoleModel.Name), e => e.Name ?? string.Empty);
        AddSortExpression(nameof(RoleModel.NormalizedName), e => e.NormalizedName ?? string.Empty);
        AddSortExpression(nameof(RoleModel.ParentRoleId), e => e.ParentRoleId ?? 0);
    }

    [HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto<PagedResult<RoleModel>>>> Get(PagedQuery query)
    {
        return await base.Get(query);
    }

    [HttpGet("{id:long}")]
    [HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto<RoleModel>>> Get(long id)
    {
        return await base.Get(id);
    }

    [HttpPost]
    [HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto<RoleModel>>> Post([FromBody] RoleModel model)
    {
        return await base.Post(model);
    }

    [HttpDelete("{id:long}")]
    [HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
    public override async Task<ActionResult<ApiResultDto>> Delete(long id)
    {
        // Prevent deletion of system roles
        var rolesResult = await Manager.Get();
        if (rolesResult is { Success: false } or { Value: null })
        {
            return StatusCode(500, new ApiResultDto(ApiResult.From(rolesResult)));
        }
        var roles = rolesResult.Value!;
        
        if (SystemRole.GetAll()
            .Join(roles, sr => sr.Name, r => r.Name, (_ , r) => r.Id)
            .Contains(id))
        { 
            var resultFailure = ApiResult.CreateFailResult("Cannot delete system role")
                .Fail("Admin role cannot be deleted");
            return BadRequest(new ApiResultDto(resultFailure));
        }

        return await base.Delete(id);
    }

    protected override Expression<Func<ApplicationRole, bool>> BuildSearchPredicate(string? search)
    {
        if (string.IsNullOrEmpty(search))
            return e => true;

        return e => e.Name!.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                   e.NormalizedName!.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
} 