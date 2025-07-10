using Microsoft.AspNetCore.Authorization;
using AuthBlocksWeb.ApiClients;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AuthBlocksWeb.HierarchicalAuthorize;

public class HierarchicalRoleRequirementHandler : AuthorizationHandler<RolesAuthorizationRequirement>
{
    private readonly IAuthApiClient _authApiClient;

    public HierarchicalRoleRequirementHandler(IAuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RolesAuthorizationRequirement requirement)
    {
        var user = context.User;
        if (!(user.Identity?.IsAuthenticated ?? true))
        {
            return;
        }

        // Get user's roles from JWT claims
        var userRoles = user.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check if user has any of the required roles or inherits from them
        foreach (var requiredRole in requirement.AllowedRoles)
        {
            if (await HasRoleOrInheritsAsync(userRoles, requiredRole))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

    private async Task<bool> HasRoleOrInheritsAsync(IList<string> userRoles, string requiredRole)
    {
        // Direct role check
        if (userRoles.Contains(requiredRole))
            return true;

        // Check if any of user's roles inherit from the required role
        foreach (var userRole in userRoles)
        {
            if (await InheritsFromRoleAsync(userRole, requiredRole))
                return true;
        }

        return false;
    }

    private async Task<bool> InheritsFromRoleAsync(string userRoleName, string targetRoleName)
    {
        try
        {
            // Get all roles from the API to check hierarchy
            var rolesResult = await _authApiClient.GetRolesAsync();
            if (!rolesResult.Success || rolesResult.Value == null)
                return false;

            var roles = rolesResult.Value;
            
            // Find the user's role
            var userRole = roles.FirstOrDefault(r => r.Name.Equals(userRoleName, StringComparison.OrdinalIgnoreCase));
            if (userRole == null)
                return false;

            // Check if user's role inherits from target role by traversing up the hierarchy
            var currentRole = userRole;
            while (currentRole.ParentRoleId.HasValue)
            {
                var parentRole = roles.FirstOrDefault(r => r.Id == currentRole.ParentRoleId.Value);
                if (parentRole == null)
                    break;

                if (parentRole.Name.Equals(targetRoleName, StringComparison.OrdinalIgnoreCase))
                    return true;

                currentRole = parentRole;
            }

            return false;
        }
        catch
        {
            // If API call fails, fall back to direct role check only
            return false;
        }
    }
} 