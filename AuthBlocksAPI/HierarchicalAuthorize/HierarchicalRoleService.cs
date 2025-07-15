using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Microsoft.Extensions.Logging;

namespace AuthBlocksAPI.HierarchicalAuthorize;

public class HierarchicalRoleService : IHierarchicalRoleService
{
    private readonly IRoleService _roleService;
    private readonly ILogger<HierarchicalRoleService> _logger;
    private readonly Dictionary<string, bool> _roleInheritanceCache = new();
    private readonly object _cacheLock = new();
    private DateTime _lastCacheRefresh = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5); // Cache for 5 minutes

    public HierarchicalRoleService(IRoleService roleService, ILogger<HierarchicalRoleService> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<bool> HasRoleOrInheritsAsync(IList<string> userRoles, string requiredRole)
    {
        // Direct role check
        if (userRoles.Contains(requiredRole))
        {
            return true;
        }

        // If user has no roles, they can't inherit anything
        if (userRoles.Count == 0)
        {
            return false;
        }

        // Check if any of user's roles inherit from the required role
        foreach (var userRole in userRoles)
        {
            if (await InheritsFromRoleAsync(userRole, requiredRole))
            {
                return true;
            }
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

            // Get all roles from the database to check hierarchy
            var rolesResult = await _roleService.Get();
            
            if (rolesResult is { Success: false } or { Value: null })
            {
                _logger.LogDebug("Roles could not be loaded");
            }
            var roles = rolesResult.Value!;
            var rolesList = roles.ToList();
            
            // Find the user's role
            var userRole = rolesList.FirstOrDefault(r => r.Name?.Equals(userRoleName, StringComparison.OrdinalIgnoreCase) == true);
            if (userRole == null)
            {
                _logger.LogDebug("User role '{UserRole}' not found in role hierarchy", userRoleName);
                return false;
            }

            // Check if user's role inherits from target role by searching down the hierarchy
            // Parent roles inherit access from their children
            var result = HasChildRole(userRole, targetRoleName, rolesList);

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
            // If database call fails, fall back to direct role check only
            return false;
        }
    }

    private bool HasChildRole(RoleModel userRole, string targetRoleName, List<RoleModel> roles)
    {
        // Check if the user's role has the target role as a direct child
        var directChildren = roles.Where(r => r.ParentRole?.Id == userRole.Id).ToList();
        
        foreach (var child in directChildren)
        {
            // Check if this child is the target role
            if (child.Name?.Equals(targetRoleName, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            
            // Recursively check if any of this child's children contain the target role
            if (HasChildRole(child, targetRoleName, roles))
            {
                return true;
            }
        }
        
        return false;
    }
} 