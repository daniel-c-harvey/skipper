using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthBlocksWeb.ApiClients;
using AuthBlocksModels.ApiModels;

namespace AuthBlocksWeb.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenService _tokenService;
    private readonly IAuthApiClient _authApiClient;
    private readonly JwtSecurityTokenHandler _jwtHandler;

    public JwtAuthenticationStateProvider(ITokenService tokenService, IAuthApiClient authApiClient)
    {
        _tokenService = tokenService;
        _authApiClient = authApiClient;
        _jwtHandler = new JwtSecurityTokenHandler();
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenService.GetAccessTokenAsync();
        
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        try
        {
            // Try to parse the JWT token
            var jwtToken = _jwtHandler.ReadJwtToken(token);
            
            // Check if token is expired
            if (jwtToken.ValidTo <= DateTime.UtcNow)
            {
                // Try to refresh the token
                var refreshResult = await TryRefreshTokenAsync();
                if (!refreshResult.success)
                {
                    await _tokenService.ClearTokensAsync();
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                
                // Use the new token
                jwtToken = _jwtHandler.ReadJwtToken(refreshResult.newToken);
            }

            // Create claims from JWT token
            var claims = jwtToken.Claims.ToList();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            // If token parsing fails, clear tokens and return anonymous user
            await _tokenService.ClearTokensAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
    public async Task<bool> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await _authApiClient.LoginAsync(loginRequest);
            if (response.Success && response.Value != null)
            {
                // Ensure localStorage operations are complete before notifying
                var token = await _tokenService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    // Pre-compute the authentication state to ensure it's ready
                    var authState = await GetAuthenticationStateAsync();
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            var response = await _authApiClient.RegisterAsync(registerRequest);
            if (response.Success && response.Value != null)
            {
                // Ensure localStorage operations are complete before notifying
                var token = await _tokenService.GetAccessTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    // Pre-compute the authentication state to ensure it's ready
                    var authState = await GetAuthenticationStateAsync();
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();
            var refreshToken = await _tokenService.GetRefreshTokenAsync();
            
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await _authApiClient.LogoutAsync(new RefreshTokenRequest 
                { 
                    AccessToken = accessToken, 
                    RefreshToken = refreshToken 
                });
            }
        }
        catch
        {
            // Continue with logout even if the API call fails
        }
        finally
        {
            await _tokenService.ClearTokensAsync();
            // Pre-compute the authentication state to ensure it's ready
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
    }

    private async Task<(bool success, string newToken)> TryRefreshTokenAsync()
    {
        try
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();
            var refreshToken = await _tokenService.GetRefreshTokenAsync();
            
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return (false, string.Empty);
            }

            var refreshRequest = new RefreshTokenRequest 
            { 
                AccessToken = accessToken, 
                RefreshToken = refreshToken 
            };

            var response = await _authApiClient.RefreshTokenAsync(refreshRequest);
            if (response.Success && response.Value != null)
            {
                // Token is saved in AuthApiClient, notify about state change
                var authState = await GetAuthenticationStateAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
                return (true, response.Value.AccessToken);
            }

            return (false, string.Empty);
        }
        catch
        {
            return (false, string.Empty);
        }
    }
} 