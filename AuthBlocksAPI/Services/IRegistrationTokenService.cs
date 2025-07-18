using AuthBlocksAPI.Models;
using AuthBlocksModels.ApiModels;

namespace AuthBlocksAPI.Services;

public interface IRegistrationTokenService
{
    Task<TokenCreationResult> GenerateTokenAsync(string pendingUserEmail);
    Task<TokenValidationResult> ValidateTokenAsync(string email, string token);
    Task<bool> ConsumeTokenAsync(string token);
}