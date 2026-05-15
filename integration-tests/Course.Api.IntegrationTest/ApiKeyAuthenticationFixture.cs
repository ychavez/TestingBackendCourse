namespace Course.Api.IntegrationTest;

public sealed class ApiKeyAuthenticationFixture
{
    public string HeaderName { get; } = "X-Api-Key";

    public string ApiKey { get; } = "integration-test-api-key";

    public void ApplyTo(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(HeaderName);
        client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderName, ApiKey);
    }
}
