using AuthBlocksAPI.Models;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
            var roles = await _roleService.GetAllRolesAsync();
            
            var roleInfos = roles.Select(r => new RoleInfo
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName ?? string.Empty,
                ParentRoleId = r.ParentRoleId,
                ParentRoleName = r.ParentRole?.Name ?? string.Empty,
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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

            // Validate parent role if specified
            ApplicationRole? parentRole = null;
            if (request.ParentRoleId.HasValue)
            {
                parentRole = await _roleService.GetActiveRoleByIdAsync(request.ParentRoleId.Value);
                if (parentRole == null)
                {
                    var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role creation failed")
                        .Fail("Parent role not found");
                    return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
                }
            }

            var role = new ApplicationRole
            {
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant(),
                ParentRoleId = parentRole?.Id,
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
                ParentRoleId = role.ParentRoleId,
                ParentRoleName = parentRole?.Name ?? string.Empty,
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
    [Authorize(Roles = "Admin")]
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

            // Validate and update parent role if specified
            if (request.ParentRoleId.HasValue && request.ParentRoleId != role.ParentRoleId)
            {
                // Prevent circular references
                if (request.ParentRoleId == id)
                {
                    var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role update failed")
                        .Fail("Role cannot be its own parent");
                    return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
                }

                var parentRole = await _roleService.GetActiveRoleByIdAsync(request.ParentRoleId.Value);
                if (parentRole == null)
                {
                    var resultFailure = ApiResult<RoleInfo>.CreateFailResult("Role update failed")
                        .Fail("Parent role not found");
                    return BadRequest(new ApiResultDto<RoleInfo>(resultFailure));
                }

                role.ParentRoleId = parentRole.Id;
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
                ParentRoleId = role.ParentRoleId,
                ParentRoleName = role.ParentRole?.Name ?? string.Empty,
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
    [Authorize(Roles = "Admin")]
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

public class CreateRoleRequest
{
    [Required]
    [StringLength(256, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    public long? ParentRoleId { get; set; }
}

public class UpdateRoleRequest
{
    [StringLength(256, MinimumLength = 1)]
    public string? Name { get; set; }
    public long? ParentRoleId { get; set; }
} 