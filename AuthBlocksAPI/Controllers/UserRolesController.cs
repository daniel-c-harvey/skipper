using API.Shared.Controllers;
using AuthBlocksData.Services;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Entities.Identity;
using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserRolesController : ModelController<ApplicationUserRole, UserRoleModel, IUserRoleService>
{
    private IUserService _userService;
    
    // Add role management endpoints
    public UserRolesController(IUserRoleService manager, IUserService userManager) : base(manager)
    {
        _userService = userManager;
    }

    [HttpPost("user/{userId:long}")]
    [Authorize(Roles = SystemRoleConstants.UserAdmin)]
    public async Task<ActionResult<ApiResultDto>> AddUserToRole([FromQuery] long userId, [FromBody] UserRoleRequest request)
    {
        try
        {
            var userResult = await _userService.GetById(userId);
            switch (userResult)
            {
                case { Success: false }:
                {
                    var resultFailure = ApiResult.From(userResult);
                    return StatusCode(500, new ApiResultDto(resultFailure));
                }
                case { Value: null }:
                {
                    var resultFailure = ApiResult.CreateFailResult("User not found")
                        .Fail("User does not exist");
                    return NotFound(new ApiResultDto(resultFailure));
                }
            }
            
            await Manager.AddUserToRoleAsync(userResult.Value, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User added to role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            var resultError = ApiResult.CreateFailResult("An error occurred while adding user to role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }

    [HttpDelete("user/{userId:long}")]
    [Authorize(Roles = SystemRoleConstants.UserAdmin)]
    public async Task<ActionResult<ApiResultDto>> RemoveUserFromRole([FromQuery] long userId, [FromBody] UserRoleRequest request)
    {
        try
        {
            var userResult = await _userService.GetById(userId);
            switch (userResult)
            {
                case { Success: false }:
                {
                    var resultFailure = ApiResult.From(userResult);
                    return StatusCode(500, new ApiResultDto(resultFailure));
                }
                case { Value: null }:
                {
                    var resultFailure = ApiResult.CreateFailResult("User not found")
                        .Fail("User does not exist");
                    return NotFound(new ApiResultDto(resultFailure));
                }
            }

            await Manager.RemoveUserFromRoleAsync(userResult.Value, request.RoleName);

            var resultSuccess = ApiResult.CreatePassResult()
                .Inform($"User removed from role '{request.RoleName}' successfully");
            return Ok(new ApiResultDto(resultSuccess));
        }
        catch (Exception ex)
        {
            var resultError = ApiResult.CreateFailResult("An error occurred while removing user from role")
                .Fail("Internal server error");
            return StatusCode(500, new ApiResultDto(resultError));
        }
    }
    
}