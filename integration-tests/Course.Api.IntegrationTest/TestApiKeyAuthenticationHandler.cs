using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Course.Api.IntegrationTest;

public static class TestApiKeyAuthenticationDefaults
{
    public const string Scheme = "TestApiKey";
}

public sealed class TestApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; set; } = "X-Api-Key";

    public string ApiKey { get; set; } = string.Empty;
}

public sealed class TestApiKeyAuthenticationHandler : AuthenticationHandler<TestApiKeyAuthenticationOptions>
{
    public TestApiKeyAuthenticationHandler(
        IOptionsMonitor<TestApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var values))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var apiKey = values.FirstOrDefault();
        if (!string.Equals(apiKey, Options.ApiKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "integration-test"),
            new Claim(ClaimTypes.Name, "Integration Test")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
