using System.Net.Http.Json;
using System.Text.Json;
using AuthBlocksModels.ApiModels;
using AuthBlocksModels.Models;
using AuthBlocksWeb.Services;
using Microsoft.Extensions.Options;
using NetBlocks.Models;

namespace AuthBlocksWeb.ApiClients;

public class PendingRegistrationClient : AuthorizingModelClient<PendingRegistrationModel, PendingRegistrationClientConfig>, IPendingRegistrationClient
{
    public PendingRegistrationClient(PendingRegistrationClientConfig config, 
                                     IOptions<JsonSerializerOptions> options, 
                                     ITokenService tokenService) 
    : base(config, options, tokenService)
    {
    }

    public async Task<TokenCreationResult> CreatePendingRegistration(string email)
    {
        try
        {
            await AddAuthorizationHeader();
            var request = new CreatePendingRegistrationRequest { Email = email };
            var response = await http.PostAsJsonAsync($"api/{config.ControllerName}/create", request, Options);
            if (response == null) throw new HttpRequestException("Failed to get response");

            var result = await response.Content.ReadFromJsonAsync<TokenCreationResult>(Options)
                ?? throw new HttpRequestException("Failed to deserialize response");

            return result;
        }
        catch (Exception e)
        {
            return TokenCreationResult.CreateFailResult(e.Message);
        }
    }
}