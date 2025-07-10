using AuthBlocksWeb.ApiClients;
using Microsoft.Extensions.Logging;

namespace AuthBlocksWeb.HierarchicalAuthorize;

public class HierarchicalRoleService : IHierarchicalRoleService
{
    private readonly IAuthApiClient _authApiClient;
    private readonly ILogger<HierarchicalRoleService> _logger;
    private readonly Dictionary<string, bool> _roleInheritanceCache = new();
    private readonly object _cacheLock = new();
    private DateTime _lastCacheRefresh = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5); // Cache for 5 minutes

    public HierarchicalRoleService(IAuthApiClient authApiClient, ILogger<HierarchicalRoleService> logger)
    {
        _authApiClient = authApiClient;
        _logger = logger;
    }

    public async Task<bool> HasRoleOrInheritsAsync(IList<string> userRoles, string requiredRole)
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
        // Create cache key
        var cacheKey = $"{userRoleName}:{targetRoleName}";
        
        // Check cache first
        lock (_cacheLock)
        {
            if (_roleInheritanceCache.TryGetValue(cacheKey, out var cachedResult))
            {
                return cachedResult;
            }
        }

        try
        {
            // Check if cache needs refresh
            if (DateTime.UtcNow - _lastCacheRefresh > _cacheExpiry)
            {
                lock (_cacheLock)
                {
                    _roleInheritanceCache.Clear();
                    _lastCacheRefresh = DateTime.UtcNow;
                }
                _logger.LogDebug("Hierarchical role cache refreshed");
            }

            // Get all roles from the API to check hierarchy
            var rolesResult = await _authApiClient.GetRolesAsync();
            if (!rolesResult.Success || rolesResult.Value == null)
            {
                _logger.LogWarning("Failed to retrieve roles from API for hierarchy check. Success: {Success}", rolesResult.Success);
                return false;
            }

            var roles = rolesResult.Value;
            
            // Find the user's role
            var userRole = roles.FirstOrDefault(r => r.Name.Equals(userRoleName, StringComparison.OrdinalIgnoreCase));
            if (userRole == null)
            {
                _logger.LogDebug("User role '{UserRole}' not found in role hierarchy", userRoleName);
                return false;
            }

            // Check if user's role inherits from target role by traversing up the hierarchy
            var currentRole = userRole;
            var result = false;
            
            while (currentRole.ParentRoleId.HasValue)
            {
                var parentRole = roles.FirstOrDefault(r => r.Id == currentRole.ParentRoleId.Value);
                if (parentRole == null)
                {
                    _logger.LogWarning("Parent role with ID {ParentRoleId} not found for role {RoleName}", 
                        currentRole.ParentRoleId.Value, currentRole.Name);
                    break;
                }

                if (parentRole.Name.Equals(targetRoleName, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }

                currentRole = parentRole;
            }

            // Cache the result
            lock (_cacheLock)
            {
                _roleInheritanceCache[cacheKey] = result;
            }

            _logger.LogDebug("Role inheritance check: {UserRole} inherits from {TargetRole}: {Result}", 
                userRoleName, targetRoleName, result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role inheritance from {UserRole} to {TargetRole}", 
                userRoleName, targetRoleName);
            // If API call fails, fall back to direct role check only
            return false;
        }
    }
}