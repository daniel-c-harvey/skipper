# Hierarchical Role Authorization for AuthBlocksAPI

This directory contains the implementation of hierarchical role authorization for the AuthBlocksAPI backend, similar to the implementation in AuthBlocksWeb but designed for API controllers.

## Overview

The hierarchical role authorization system allows you to define role hierarchies where parent roles inherit permissions from their child roles. This means that if a user has a parent role, they automatically have access to resources that require any of their child roles.

## Components

### 1. IHierarchicalRoleService
Interface that defines the contract for checking hierarchical role permissions.

### 2. HierarchicalRoleService
Implementation that checks role hierarchies by querying the database directly through the RoleService.

### 3. HierarchicalRolesAuthorizationHandler
ASP.NET Core authorization handler that integrates with the authorization pipeline to check hierarchical role permissions.

### 4. HierarchicalRoleAuthorizeAttribute
Custom authorization attribute that can be used on controllers and actions, similar to the standard `[Authorize]` attribute but with hierarchical role support.

## Usage

### Basic Usage

Replace the standard `[Authorize]` attribute with `[HierarchicalRoleAuthorize]`:

```csharp
// Before (standard authorization)
[Authorize(Roles = SystemRoleConstants.UserAdmin)]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)

// After (hierarchical authorization)
[HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)
```

### Multiple Roles

You can specify multiple roles, and the user will be authorized if they have any of the specified roles or inherit from them:

```csharp
[HierarchicalRoleAuthorize(SystemRoleConstants.Admin, SystemRoleConstants.UserAdmin)]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)
```

### Authentication Only

If you don't specify any roles, it works like the standard `[Authorize]` attribute - just requires authentication:

```csharp
[HierarchicalRoleAuthorize]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> GetRole(long id)
```

## Role Hierarchy Example

Based on the SystemRole definitions:

- **Admin** (parent role)
  - **UserAdmin** (child role)

In this hierarchy:
- A user with the "Admin" role can access resources that require "UserAdmin" role
- A user with only "UserAdmin" role cannot access resources that require "Admin" role

## How It Works

1. When a request is made to an endpoint with `[HierarchicalRoleAuthorize]`, the authorization handler is triggered
2. The handler extracts the user's roles from JWT claims
3. For each required role, it checks if the user has that role directly or inherits from it
4. The hierarchical check is done by querying the database to find the role hierarchy
5. Results are cached for 5 minutes to improve performance

## Performance Considerations

- Role hierarchy checks are cached for 5 minutes to reduce database queries
- The cache is automatically refreshed when it expires
- Direct role matches are checked first before doing hierarchical lookups

## Configuration

The hierarchical authorization services are automatically registered in `Program.cs`:

```csharp
// Add Hierarchical Role Authorization services
builder.Services.AddScoped<IHierarchicalRoleService, HierarchicalRoleService>();
builder.Services.AddScoped<IAuthorizationHandler, HierarchicalRolesAuthorizationHandler>();
```

## Logging

The system includes comprehensive logging at the Debug level to help troubleshoot authorization issues. Check the logs for:

- User authentication status
- User roles from JWT claims
- Required roles for the endpoint
- Role inheritance check results
- Authorization success/failure

## Migration from Standard Authorization

To migrate from standard `[Authorize]` to hierarchical authorization:

1. Add the using statement: `using AuthBlocksAPI.HierarchicalAuthorize;`
2. Replace `[Authorize(Roles = "...")]` with `[HierarchicalRoleAuthorize("...")]`
3. The behavior will be the same for direct role matches, but now includes hierarchical inheritance

## Example Migration

```csharp
// Before
[Authorize(Roles = SystemRoleConstants.UserAdmin)]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)

// After
[HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)
```

The functionality remains the same, but now users with the "Admin" role (which is a parent of "UserAdmin") will also be able to access this endpoint. 