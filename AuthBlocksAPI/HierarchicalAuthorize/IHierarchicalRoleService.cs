namespace AuthBlocksAPI.HierarchicalAuthorize;

public interface IHierarchicalRoleService
{
    Task<bool> HasRoleOrInheritsAsync(IList<string> userRoles, string requiredRole);
} 