using API.Shared.Controllers;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksAPI.Models;
using AuthBlocksAPI.Services;
using AuthBlocksData.Services;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Mvc;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
public class PendingRegistrationController : BaseModelController<PendingRegistration, PendingRegistrationModel, IPendingRegistrationService>
{
    private readonly IRegistrationTokenService _tokenService;

    public PendingRegistrationController(IRegistrationTokenService tokenService, IPendingRegistrationService manager) : base(manager)
    {
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResultDto<TokenCreationResultDto>>> Create([FromBody] CreatePendingRegistrationRequest model)
    {
        var result = await _tokenService.GenerateTokenAsync(model.Email);
        var resultDto = new TokenCreationResultDto(result);

        return (result.Success)
            ? Ok(resultDto)
            : StatusCode(500, resultDto);
    }
}