using AuthBlocksAPI.Models;
using AuthBlocksAPI.Services;
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
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserService userService,
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _userManager = userManager;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResultDto<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                var emailresult = ApiResult<AuthResponse>.CreateFailResult("Invalid email or password")
                    .Fail("User not found");
                return BadRequest(new ApiResultDto<AuthResponse>(emailresult));
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                var passwordResult = ApiResult<AuthResponse>.CreateFailResult("Invalid email or password")
                    .Fail("Invalid password");
                return BadRequest(new ApiResultDto<AuthResponse>(passwordResult));
            }

            var accessToken = await _jwtService.GenerateTokenAsync(user);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(refreshToken, user.Id);

            var userRoles = await _userService.GetActiveUserRolesAsync(user);

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
                    Roles = userRoles.ToList()
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

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = true, // For API, we'll skip email confirmation
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow
            };

            var createResult = await _userService.CreateUserAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("Registration failed");
                foreach (var error in createResult.Errors)
                {
                    resultFailure.Fail(error.Description);
                }
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            var accessToken = await _jwtService.GenerateTokenAsync(user);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(refreshToken, user.Id);

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
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
            var resultError = ApiResult<AuthResponse>.CreateFailResult("An error occurred during registration")
                .Fail("Internal server error");
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

            var user = await _userService.GetActiveUserByIdAsync(userId);
            if (user == null)
            {
                var resultFailure = ApiResult<AuthResponse>.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return BadRequest(new ApiResultDto<AuthResponse>(resultFailure));
            }

            // Revoke old refresh token and generate new tokens
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
            
            var newAccessToken = await _jwtService.GenerateTokenAsync(user);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync();
            await _jwtService.SaveRefreshTokenAsync(newRefreshToken, user.Id);

            var userRoles = await _userService.GetActiveUserRolesAsync(user);

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
                    Roles = userRoles.ToList()
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

            var user = await _userService.GetActiveUserByIdAsync(userId);
            if (user == null)
            {
                var resultFailure = ApiResult<UserInfo>.CreateFailResult("User not found")
                    .Fail("User does not exist");
                return NotFound(new ApiResultDto<UserInfo>(resultFailure));
            }

            var userRoles = await _userService.GetActiveUserRolesAsync(user);

            var userInfo = new UserInfo
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = userRoles.ToList()
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
} 