using Microsoft.AspNetCore.Authentication;

namespace BankTransfer.Api.Options;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
    public const string HeaderName = "x-api-key";
}