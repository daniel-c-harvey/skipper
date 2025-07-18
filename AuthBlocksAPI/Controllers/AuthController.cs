using AuthBlocksAPI.Services;
using AuthBlocksData.Services;
using AuthBlocksModels.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Models;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserService userService,
        IRoleService roleService,
        IUserRoleService userRoleService,
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _userRoleService = userRoleService;
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResultDto<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Use the manager instead of the service to get the entity with password hash
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var emailResult = ApiResult<AuthResponse>.CreateFailResult("Invalid email or password");
                return Ok(new ApiResultDto<AuthResponse>(emailResult));
            }

            if (user.IsDeactivated)
            {
                var deactivatedResult = ApiResult<AuthResponse>.CreateFailResult("User account is deactivated")
                    .Inform("Please contact the administrator to reactivate your account.");
                return Ok(new ApiResultDto<AuthResponse>(deactivatedResult));
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                var passwordResult = ApiResult<AuthResponse>.CreateFailResult("Invalid email or password");
                return Ok(new ApiResultDto<AuthResponse>(passwordResult));
            }

            var userModel = UserEntityToModelConverter.Convert(user);
            
            var accessToken = await _jwtService.GenerateTokenAsync(userModel);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(refreshToken, user.Id);

            var rolesResult = await _userRoleService.GetByUser(userModel);
            if (rolesResult is null or { Success: false } or { Value: null })
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Login failed")
                    .Fail("User roles could not be determined");
                return Ok(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var roles = rolesResult.Value!;
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Match JWT expiry
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.Select(r => r.Name!).ToList()
                }
            };

            var resultSuccess = ApiResult<AuthResponse>.CreatePassResult(response)
                .Inform("Login successful");
            return Ok(new ApiResultDto<AuthResponse>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Email}", request.Email);
            var result = ApiResult<AuthResponse>.CreateFailResult("An error occurred during login")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<AuthResponse>(result));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResultDto<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var existingUser = await _userService.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Registration failed")
                    .Fail("User with this email already exists");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var user = new UserModel
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = true, // For API, we'll skip email confirmation
                NormalizedUserName = request.UserName.ToUpper(),
                NormalizedEmail = request.Email.ToUpper(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userService.Add(user, request.Password);
            if (!createResult.Success)
            {
                var resultFailure = ApiResult<AuthResponse>.From(createResult);
                return StatusCode(500, new ApiResultDto<AuthResponse>(resultFailure));
            }

            // Use the returned user from createResult instead of fetching again
            var createdUser = createResult.Value!;

            var accessToken = await _jwtService.GenerateTokenAsync(createdUser);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(refreshToken, createdUser.Id);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserInfo
                {
                    Id = createdUser.Id,
                    UserName = createdUser.UserName ?? string.Empty,
                    Email = createdUser.Email ?? string.Empty,
                    Roles = new List<string>() // New user has no roles initially
                }
            };

            var resultSuccess = ApiResult<AuthResponse>.CreatePassResult(response)
                .Inform("Registration successful");
            return Ok(new ApiResultDto<AuthResponse>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Email}", request.Email);
            var resultError = ApiResult<AuthResponse>.CreateFailResult("An error occurred during registration");
            return StatusCode(500, new ApiResultDto<AuthResponse>(resultError));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResultDto<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var principal = _jwtService.ValidateToken(request.AccessToken);
            if (principal == null)
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Invalid access token")
                    .Fail("Token validation failed");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Invalid token claims")
                    .Fail("User ID not found in token");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var isValidRefreshToken = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, userId);
            if (!isValidRefreshToken)
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Invalid refresh token")
                    .Fail("Refresh token validation failed");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var userResult = await _userService.GetById(userId);
            switch (userResult)
            {
                case {Success: false}:
                {
                    return StatusCode(500, new ApiResultDto<AuthResponse>(ApiResult<AuthResponse>.From(userResult)));
                }
                case {Value: null}:
                    var resultFailure = ApiResult<AuthResponse>.CreateFailResult("User not found");
                    return StatusCode(500, new ApiResultDto<AuthResponse>(resultFailure));
            }

            var user = userResult.Value!;
            var userEntity = UserEntityToModelConverter.Convert(user);

            // Revoke old refresh token and generate new tokens
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            var newAccessToken = await _jwtService.GenerateTokenAsync(user);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(newRefreshToken, user.Id);

            var rolesResult = await _userRoleService.GetByUser(user);
            if (rolesResult is null or { Success: false } or { Value: null })
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Refresh failed")
                    .Fail("User roles could not be determined");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }
            
            var roles = rolesResult.Value!;
            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.Select(r => r.Name!).ToList()
                }
            };

            var resultSuccess = ApiResult<AuthResponse>.CreatePassResult(response)
                .Inform("Token refreshed successfully");
            return Ok(new ApiResultDto<AuthResponse>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            var resultError = ApiResult<AuthResponse>.CreateFailResult("An error occurred during token refresh")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<AuthResponse>(resultError));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResultDto>> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            var resultSuccess = ApiResult.CreatePassResult().Inform("Logout successful");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            var resultError = ApiResult.CreateFailResult("An error occurred during logout")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResultDto<UserInfo>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("Invalid user claims")
                    .Fail("User ID not found in token");
                return BadRequest(new ApiResultDto<UserInfo>(resultFailure));
            }

            var userResult = await _userService.GetById(userId);
            if (userResult is {Success: false} or {Value: null})
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto<UserInfo>(resultFailure));
            }
            
            var user = userResult.Value!;
            var rolesResult = await _userRoleService.GetByUser(user);
            if (rolesResult is { Success: false } or { Value: null })
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Refresh failed")
                    .Fail("User roles could not be determined");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }
            
            var roles = rolesResult.Value!;

            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.Select(r => r.Name!).ToList()
            };

            var resultSuccess = ApiResult<UserInfo>.CreatePassResult(userInfo)
                .Inform("User info retrieved successfully");
            return Ok(new ApiResultDto<UserInfo>(resultSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user info");
            var resultError = ApiResult<UserInfo>.CreateFailResult("An error occurred while retrieving user info")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto<UserInfo>(resultError));
        }
    }
    [HttpGet("roles")]
    public async Task<ActionResult<ApiResultDto<List<RoleInfo>>>> GetRoles()
    {
        try
        {
            var rolesResult = await _roleService.Get();

            if (rolesResult is { Success: false } or { Value: null })
            {
                return StatusCode(500, new ApiResultDto<List<RoleInfo>>(ApiResult<List<RoleInfo>>.From(rolesResult)));
            }
            var roles = rolesResult.Value!;
            
            var roleInfos = roles.Select(r => new RoleInfo
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName,
                ParentRoleName = r.ParentRole?.Name,
                ParentRoleId = r.ParentRole?.Id,
                Created = r.CreatedAt,
                Modified = r.UpdatedAt
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
} 