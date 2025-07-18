using System.Security.Cryptography;
using System.Text;
using AuthBlocksAPI.Models;
using AuthBlocksData.Data;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthBlocksAPI.Services;

public class RegistrationTokenService : IRegistrationTokenService
{
    private readonly AuthDbContext _context;
    private readonly ILogger<RegistrationTokenService> _logger;
    private const string CharacterSet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int TokenLength = 10;
    private static readonly TimeSpan TokenExpiration = TimeSpan.FromDays(7);
    
    public RegistrationTokenService(AuthDbContext context, ILogger<RegistrationTokenService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<TokenCreationResult> GenerateTokenAsync(string pendingUserEmail)
    {
        try
        {
            var token = GenerateRandomToken();
            var hashedToken = HashToken(token);
        
            var pendingRegistration = new PendingRegistration()
            {
                TokenHash = hashedToken,
                PendingUserEmail = pendingUserEmail,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(TokenExpiration),
                IsConsumed = false
            };

            _context.Set<PendingRegistration>().Add(pendingRegistration);
            await _context.SaveChangesAsync();
        
            _logger.LogInformation("Generated registration token for pending user {pendingUserEmail}", pendingUserEmail);
        
            return TokenCreationResult.CreatePassResult(pendingUserEmail, token);
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
        var hashedToken = HashToken(normalizedToken);
        
        var registrationToken = await _context.Set<PendingRegistration>()
            .FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);
        
        if (registrationToken == null)
        {
            return TokenValidationResult.CreateFailResult("Invalid registration token");
        }
        
        if (registrationToken.ExpiresAt < DateTime.UtcNow)
        {
            return TokenValidationResult.CreateFailResult("Registration token has expired");
        }

        if (registrationToken.IsConsumed)
        {
            return TokenValidationResult.CreateFailResult("Registration token has already been consumed");
        }

        if (!registrationToken.PendingUserEmail.Equals(email))
        {
            return TokenValidationResult.CreateFailResult("Invalid registration token");
        }
        
        return new TokenValidationResult(registrationToken.Id, registrationToken.IsConsumed);
    }

    public async Task<bool> ConsumeTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        var normalizedToken = token.Trim().ToUpperInvariant();
        var hashedToken = HashToken(normalizedToken);
        
        var registrationToken = await _context.Set<PendingRegistration>()
            .FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken);
        
        if (registrationToken == null || 
            registrationToken.ExpiresAt < DateTime.UtcNow || 
            registrationToken.IsConsumed)
        {
            return false;
        }
        
        registrationToken.IsConsumed = true;
        registrationToken.ConsumedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Registration token consumed for pending user {PendingUserEmail}", registrationToken.PendingUserEmail);
        
        return true;
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

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashedBytes);
    }
}