using AuthBlocksAPI.Models;
using AuthBlocksModels.ApiModels;
using NetBlocks.Models;

namespace AuthBlocksAPI.Services;

public interface IRegistrationTokenService
{
    Task<TokenCreationResult> GenerateTokenAsync(string pendingUserEmail);
    Task<TokenValidationResult> ValidateTokenAsync(string email, string token);
    Task<Result> ConsumeTokenAsync(string email, string token);
}