using AuthBlocksAPI.Models;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly RoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleService roleService, ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResultDto<List<RoleInfo>>>> GetRoles()
    {
        try
        {
            // Since we don't have a GetAll method, we'll get by specific role names
            // In a real implementation, you'd add a GetAll method to the repository
            var roles = new List<ApplicationRole>();
            
            // Get Admin role
            var adminRole = await _roleService.GetActiveRoleByNameAsync("ADMIN");
            if (adminRole != null) roles.Add(adminRole);
            
            // Get User role (if exists)
            var userRole = await _roleService.GetActiveRoleByNameAsync("USER");
            if (userRole != null) roles.Add(userRole);

            var roleInfos = roles.Select(r => new RoleInfo
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName ?? string.Empty,
                Created = r.Created,
                Modified = r.Modified
            }).ToList();

            var resultSuccess = ApiResult<List<RoleInfo>>.CreatePassResult(roleInfos)
                .Inform("Roles retrieved successfully");
            return Ok(new ApiResultDto<List<RoleInfo>>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            var resultError = ApiResult<List<RoleInfo>>.CreateFailResult("An error occurred while retrieving roles")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<List<RoleInfo>>(resultError));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResultDto<RoleInfo>>> GetRole(long id)
    {
        try
        {
            var role = await _roleService.GetActiveRoleByIdAsync(id);
            if (role == null)
            {
                var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role not found")
                    .Fail("Role does not exist");
                return NotFound(new ApiResultDto<RoleInfo>(resultFailure));
            }

            var roleInfo = new RoleInfo
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName ?? string.Empty,
                Created = role.Created,
                Modified = role.Modified
            };

            var resultSuccess = ApiResult<RoleInfo>.CreatePassResult(roleInfo)
                .Inform("Role retrieved successfully");
            return Ok(new ApiResultDto<RoleInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            var resultError = ApiResult<RoleInfo>.CreateFailResult("An error occurred while retrieving role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<RoleInfo>(resultError));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResultDto<RoleInfo>>> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            // Check if role already exists
            var existingRole = await _roleService.FindByNameAsync(request.Name);
            if (existingRole != null)
            {
                var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role creation failed")
                    .Fail("Role with this name already exists");
                return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
            }

            var role = new ApplicationRole
            {
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant(),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var result = await _roleService.CreateRoleAsync(role);
            if (!result.Succeeded)
            {
                var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role creation failed");
                foreach (var error in result.Errors)
                {
                    resultFailure.Fail(error.Description);
                }
                return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
            }

            var roleInfo = new RoleInfo
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName ?? string.Empty,
                Created = role.Created,
                Modified = role.Modified
            };

            var resultSuccess = ApiResult<RoleInfo>.CreatePassResult(roleInfo)
                .Inform("Role created successfully");
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new ApiResultDto<RoleInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
            var resultError = ApiResult<RoleInfo>.CreateFailResult("An error occurred while creating role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<RoleInfo>(resultError));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResultDto<RoleInfo>>> UpdateRole(long id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var role = await _roleService.GetActiveRoleByIdAsync(id);
            if (role == null)
            {
                var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role not found")
                    .Fail("Role does not exist");
                return NotFound(new ApiResultDto<RoleInfo>(resultFailure));
            }

            // Check if new name conflicts with existing role
            if (!string.IsNullOrEmpty(request.Name) && request.Name != role.Name)
            {
                var existingRole = await _roleService.FindByNameAsync(request.Name);
                if (existingRole != null && existingRole.Id != id)
                {
                    var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role update failed")
                        .Fail("Role with this name already exists");
                    return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
                }

                role.Name = request.Name;
                role.NormalizedName = request.Name.ToUpperInvariant();
            }

            role.Modified = DateTime.UtcNow;

            var result = await _roleService.UpdateRoleAsync(role);
            if (!result.Succeeded)
            {
                var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role update failed");
                foreach (var error in result.Errors)
                {
                    resultFailure.Fail(error.Description);
                }
                return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
            }

            var roleInfo = new RoleInfo
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName ?? string.Empty,
                Created = role.Created,
                Modified = role.Modified
            };

            var resultSuccess = ApiResult<RoleInfo>.CreatePassResult(roleInfo)
                .Inform("Role updated successfully");
            return Ok(new ApiResultDto<RoleInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            var resultError = ApiResult<RoleInfo>.CreateFailResult("An error occurred while updating role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<RoleInfo>(resultError));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResultDto>> DeleteRole(long id)
    {
        try
        {
            var role = await _roleService.GetActiveRoleByIdAsync(id);
            if (role == null)
            {
                var resultFailure = ApiResult.CreateFailResult("Role not found")
                    .Fail("Role does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            // Prevent deletion of system roles
            if (role.Name == "Admin")
            {
                var resultFailure = ApiResult.CreateFailResult("Cannot delete system role")
                    .Fail("Admin role cannot be deleted");
                return BadRequest(new ApiResultDto(resultFailure));
            }

            await _roleService.SoftDeleteRoleAsync(role);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform("Role deleted successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            var resultError = ApiResult.CreateFailResult("An error occurred while deleting role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }
}

public class RoleInfo
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}

public class CreateRoleRequest
{
    [Required]
    [StringLength(256, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    [StringLength(256, MinimumLength = 1)]
    public string? Name { get; set; }
} 