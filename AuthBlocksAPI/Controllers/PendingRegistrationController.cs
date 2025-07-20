using API.Shared.Common.Email.Mailtrap;
using API.Shared.Controllers;
using AuthBlocksAPI.Common;
using AuthBlocksAPI.HierarchicalAuthorize;
using AuthBlocksAPI.Services;
using AuthBlocksData.Services;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities;
using AuthBlocksModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NetBlocks.Models;

namespace AuthBlocksAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[HierarchicalRoleAuthorize(SystemRoleConstants.UserAdmin)]
public class PendingRegistrationController : BaseModelController<PendingRegistration, PendingRegistrationModel, IPendingRegistrationService>
{
    private readonly IRegistrationTokenService _tokenService;
    private readonly IUserService _userService;
    private readonly IGeneralEmailSender _emailSender;

    public PendingRegistrationController(IRegistrationTokenService tokenService, 
                                         IPendingRegistrationService manager, 
                                         IUserService userService,
                                         IGeneralEmailSender emailSender)
    : base(manager)
    {
        _tokenService = tokenService;
        _userService = userService;
        _emailSender = emailSender;
    }

    [HttpPost("create")]
    public async Task<ActionResult<RegistrationCreatedResult.RegistrationCreatedResultDto>> Create([FromBody] CreatePendingRegistrationRequest model)
    {
        var existingUser = await _userService.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            var resultFailure = RegistrationCreatedResult.CreateFailResult("User with this email already exists");
            return BadRequest(new RegistrationCreatedResult.RegistrationCreatedResultDto(resultFailure));
        }
        
        var existingRegistrationResult = await Manager.FindByEmail(model.Email);
        if (existingRegistrationResult is { Value: PendingRegistrationModel existingRegistration })
        {
            var resultFailure = RegistrationCreatedResult.CreateFailResult("User with this email already pending registration");
            return BadRequest(new RegistrationCreatedResult.RegistrationCreatedResultDto(resultFailure));
        }
        
        var tokenResult = await _tokenService.GenerateTokenAsync(model.Email);

        if (tokenResult is {
                            Success: true, 
                            RegistrationEmail: string email,            
                            RegistrationToken: string token, 
                            RegistrationTokenHash: string hash,
                            TokenExpiration: TimeSpan expiration,
                           })
        {
            var pendingRegistration = new PendingRegistrationModel()
            {
                PendingUserEmail = email,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(expiration),
                IsConsumed = false,
                Roles = model.Roles
            };

            await Manager.Create(hash, pendingRegistration);
            
            string subject = "[Skipper ERP] Register New Account";
            string link =  QueryHelpers.AddQueryString(model.ReturnHost, new Dictionary<string, string?>
            {
                ["UserEmail"] = email,
                ["RegistrationToken"] = token
            });
            string message = RegistrationEmailTemplate.Create(token, link);
            await _emailSender.SendEmailAsync(email, null, subject, message);
        }
        var result = RegistrationCreatedResult.From(tokenResult, tokenResult.RegistrationEmail);
        var resultDto = new RegistrationCreatedResult.RegistrationCreatedResultDto(result);

        return (tokenResult.Success)
            ? Ok(resultDto)
            : StatusCode(500, resultDto);
    }
}