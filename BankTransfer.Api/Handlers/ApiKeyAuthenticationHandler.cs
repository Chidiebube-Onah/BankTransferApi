
using System.Security.Claims;
using System.Text.Encodings.Web;
using BankTransfer.Api.Options;
using BankTransfer.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RestSharp;

namespace BankTransfer.Api.Handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyCacheService _apiKeyCacheService;
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyCacheService apiKeyCacheService) : base(options, logger, encoder, clock)
    {
        _apiKeyCacheService = apiKeyCacheService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out StringValues apiKey) || apiKey.Count != 1)
        {
            Logger.LogWarning("An API request was received without the x-api-key header");
            return AuthenticateResult.Fail("Invalid parameters");
        }

        Guid clientId = await _apiKeyCacheService.GetClientIdFromApiKey(apiKey);

        if (clientId == default)
        {
            Logger.LogWarning($"An API request was received with an invalid API key: {apiKey}");
            return AuthenticateResult.Fail("Invalid parameters");
        }

        Logger.BeginScope("{ClientId}", clientId);
        Logger.LogInformation("Client authenticated");

        Claim[] claims = new[] { new Claim(ClaimTypes.Name, clientId.ToString()) };
        ClaimsIdentity identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
        List<ClaimsIdentity> identities = new List<ClaimsIdentity> { identity };
        ClaimsPrincipal principal = new ClaimsPrincipal(identities);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

        return AuthenticateResult.Success(ticket);
    }

}