using Microsoft.AspNetCore.Authorization;

namespace AuthBlocksAPI.HierarchicalAuthorize;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HierarchicalRoleAuthorizeAttribute : AuthorizeAttribute
{
    public HierarchicalRoleAuthorizeAttribute(params string[]? roles)
    {
        // Only set Roles if roles are actually specified
        // If no roles are specified, it works like [Authorize] - just requires authentication
        if (roles != null && roles.Length > 0)
        {
            Roles = string.Join(",", roles);
        }
        // If roles is null or empty, don't set Roles property - this makes it work like [Authorize]
    }
} 