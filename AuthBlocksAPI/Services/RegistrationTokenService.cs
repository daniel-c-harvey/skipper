using System.Security.Cryptography;
using System.Text;
using AuthBlocksAPI.Models;
using AuthBlocksData.Data;
using AuthBlocksData.Data.Repositories;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Converters;
using AuthBlocksModels.Entities;
using Microsoft.EntityFrameworkCore;
using NetBlocks.Models;

namespace AuthBlocksAPI.Services;

public class RegistrationTokenService : IRegistrationTokenService
{
    private readonly ILogger<RegistrationTokenService> _logger;
    private readonly IPendingRegistrationRepository _repository;
    private const string CharacterSet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int TokenLength = 10;
    private static readonly TimeSpan TokenExpiration = TimeSpan.FromDays(7);
    
    public RegistrationTokenService(IPendingRegistrationRepository repository, ILogger<RegistrationTokenService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<TokenCreationResult> GenerateTokenAsync(string pendingUserEmail)
    {
        try
        {
            var token = GenerateRandomToken();
            var hashedToken = HashToken(pendingUserEmail, token);
            
            return TokenCreationResult.CreatePassResult(pendingUserEmail, token, hashedToken, TokenExpiration);
        }
        catch (Exception e)
        {
            return TokenCreationResult.CreateFailResult(e.Message);
        }
    }
    
    public async Task<TokenValidationResult> ValidateTokenAsync(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return TokenValidationResult.CreateFailResult("Token cannot be empty");
        }

        var normalizedToken = token.Trim().ToUpperInvariant();
        var hashedToken = HashToken(email, normalizedToken);

        var pendingRegistration = (await _repository.FindAsync(rt => rt.PendingUserEmail == email && !rt.IsConsumed)).FirstOrDefault();
        
        if (pendingRegistration == null)
        {
            return TokenValidationResult.CreateFailResult("Invalid registration token");
        }
        
        if (pendingRegistration.ExpiresAt < DateTime.UtcNow)
        {
            return TokenValidationResult.CreateFailResult("Registration token has expired");
        }

        if (pendingRegistration.IsConsumed)
        {
            return TokenValidationResult.CreateFailResult("Registration token has already been consumed");
        }
        
        return new TokenValidationResult(pendingRegistration.Id, 
                                         pendingRegistration.IsConsumed, 
                                         pendingRegistration.Roles?.Select(RoleEntityToModelConverter.Convert));
    }

    public async Task<Result> ConsumeTokenAsync(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result.CreateFailResult("Token cannot be empty");

        var normalizedToken = token.Trim().ToUpperInvariant();
        var hashedToken = HashToken(email, normalizedToken);

        var registrationToken = (await _repository.FindAsync(rt => rt.TokenHash == hashedToken)).FirstOrDefault();
        
        if (registrationToken == null || 
            registrationToken.ExpiresAt < DateTime.UtcNow || 
            registrationToken.IsConsumed)
        {
            return Result.CreateFailResult("Invalid registration token");
        }
        
        registrationToken.IsConsumed = true;
        registrationToken.ConsumedAt = DateTime.UtcNow;
        
        await _repository.UpdateAsync(registrationToken);
        
        _logger.LogInformation("Registration token consumed for pending user {PendingUserEmail}", registrationToken.PendingUserEmail);
        
        return Result.CreatePassResult();
    }

    private static string GenerateRandomToken()
    {
        var buffer = new byte[TokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        
        var token = new StringBuilder(TokenLength);
        
        for (int i = 0; i < TokenLength; i++)
        {
            token.Append(CharacterSet[buffer[i] % CharacterSet.Length]);
        }
        
        return token.ToString();
    }

    private static string HashToken(string email, string token)
    {
        using var sha256 = SHA256.Create();
        
        {
            var bytes = Encoding.UTF8.GetBytes($"{email}::{token}");
            var hashedBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}