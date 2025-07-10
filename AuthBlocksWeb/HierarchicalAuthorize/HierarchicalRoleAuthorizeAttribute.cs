using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AuthBlocksWeb.HierarchicalAuthorize;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HierarchicalRoleAuthorizeAttribute : AuthorizeAttribute
{
    public HierarchicalRoleAuthorizeAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}

public class HierarchicalRoleAuthorizeView : AuthorizeView
{
    [Parameter]
    public string[] RolesList { get; set; } = [];

    protected override void OnParametersSet()
    {
        // Use standard AuthorizeView behavior with roles
        Roles = RolesList.Any() ? string.Join(',', RolesList) : null;
        base.OnParametersSet();
    }
} 