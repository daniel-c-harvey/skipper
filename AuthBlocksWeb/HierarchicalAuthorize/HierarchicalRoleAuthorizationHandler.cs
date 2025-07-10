using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AuthBlocksWeb.HierarchicalAuthorize;

public class HierarchicalRoleRequirementHandler : AuthorizationHandler<HierarchicalRoleRequirement>
{
    private readonly IHierarchicalRoleService _hierarchicalRoleService;

    public HierarchicalRoleRequirementHandler(IHierarchicalRoleService hierarchicalRoleService)
    {
        _hierarchicalRoleService = hierarchicalRoleService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HierarchicalRoleRequirement requirement)
    {
        var user = context.User;
        if (!(user.Identity?.IsAuthenticated ?? false))
        {
            return;
        }

        // Get user's roles from JWT claims
        var userRoles = user.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check if user has any of the required roles or inherits from them
        foreach (var requiredRole in requirement.RequiredRoles)
        {
            if (await _hierarchicalRoleService.HasRoleOrInheritsAsync(userRoles, requiredRole))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
} 