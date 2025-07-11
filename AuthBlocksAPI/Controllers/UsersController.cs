using AuthBlocksAPI.Models;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        UserManager<ApplicationUser> userManager,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResultDto<List<UserInfo>>>> GetUsers()
    {
        try
        {
            // For now, we'll get users by role since we don't have a GetAll method
            // In a real implementation, you'd add pagination and filtering
            var allUsers = await _userService.GetActiveUsersInRoleAsync("Admin");
            var regularUsers = await _userService.GetActiveUsersInRoleAsync("User");
            
            var combinedUsers = allUsers.Concat(regularUsers).Distinct().ToList();
            
            var userInfos = new List<UserInfo>();
            
            foreach (var user in combinedUsers)
            {
                var roles = await _userService.GetActiveUserRolesAsync(user);
                userInfos.Add(new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            var resultSuccess = ApiResult<List<UserInfo>>.CreatePassResult(userInfos)
                .Inform("Users retrieved successfully");
            return Ok(new ApiResultDto<List<UserInfo>>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            var resultError = ApiResult<List<UserInfo>>.CreateFailResult("An error occurred while retrieving users")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<List<UserInfo>>(resultError));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResultDto<UserInfo>>> GetUser(long id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Users can only view their own profile unless they're admin
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto<UserInfo>(resultFailure));
            }

            var roles = await _userService.GetActiveUserRolesAsync(user);
            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };

            var resultSuccess = ApiResult<UserInfo>.CreatePassResult(userInfo)
                .Inform("User retrieved successfully");
            return Ok(new ApiResultDto<UserInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            var resultError = ApiResult<UserInfo>.CreateFailResult("An error occurred while retrieving user")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<UserInfo>(resultError));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResultDto<UserInfo>>> UpdateUser(long id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Users can only update their own profile unless they're admin
            if (currentUserId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto<UserInfo>(resultFailure));
            }

            // Update user properties
            user.UserName = request.UserName ?? user.UserName;
            user.Email = request.Email ?? user.Email;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userService.UpdateUserAsync(user);
            if (!updateResult.Succeeded)
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("Update failed");
                foreach (var error in updateResult.Errors)
                {
                    resultFailure.Fail(error.Description);
                }
                return BadRequest(new ApiResultDto<UserInfo>(resultFailure));
            }

            var roles = await _userService.GetActiveUserRolesAsync(user);
            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };

            var resultSuccess = ApiResult<UserInfo>.CreatePassResult(userInfo)
                .Inform("User updated successfully");
            return Ok(new ApiResultDto<UserInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            var resultError = ApiResult<UserInfo>.CreateFailResult("An error occurred while updating user")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<UserInfo>(resultError));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResultDto>> DeleteUser(long id)
    {
        try
        {
            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            // Prevent admin from deleting themselves
            var currentUserId = GetCurrentUserId();
            if (currentUserId == id)
            {
                var resultFailure = ApiResult.CreateFailResult("Cannot delete your own account")
                    .Fail("Self-deletion not allowed");
                return BadRequest(new ApiResultDto(resultFailure));
            }

            await _userService.SoftDeleteUserAsync(user);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform("User deleted successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            var resultError = ApiResult.CreateFailResult("An error occurred while deleting user")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    [HttpPost("{id}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResultDto>> AddUserToRole(long id, [FromBody] UserRoleRequest request)
    {
        try
        {
            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            await _userService.AddUserToRoleAsync(user, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User added to role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to role {RoleName}", id, request.RoleName);
            var resultError = ApiResult.CreateFailResult("An error occurred while adding user to role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    [HttpDelete("{id}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResultDto>> RemoveUserFromRole(long id, [FromBody] UserRoleRequest request)
    {
        try
        {
            var user = await _userService.GetActiveUserByIdAsync(id);
            if (user == null)
            {
                var resultFailure = ApiResult.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto(resultFailure));
            }

            await _userService.RemoveUserFromRoleAsync(user, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User removed from role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId} from role {RoleName}", id, request.RoleName);
            var resultError = ApiResult.CreateFailResult("An error occurred while removing user from role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    private long GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdClaim?.Value, out var userId) ? userId : 0;
    }
} 