using Course.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Course.Api.IntegrationTest;

public sealed class CourseApiFactory : WebApplicationFactory<Program>
{
    public CourseApiTestDataFixture TestData { get; } = new();

    public ApiKeyAuthenticationFixture ApiKeyAuthentication { get; } = new();

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        ApiKeyAuthentication.ApplyTo(client);

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<InMemoryStore>();
            services.AddSingleton(_ => TestData.CreateStore());

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestApiKeyAuthenticationDefaults.Scheme;
                    options.DefaultChallengeScheme = TestApiKeyAuthenticationDefaults.Scheme;
                })
                .AddScheme<TestApiKeyAuthenticationOptions, TestApiKeyAuthenticationHandler>(
                    TestApiKeyAuthenticationDefaults.Scheme,
                    options =>
                    {
                        options.HeaderName = ApiKeyAuthentication.HeaderName;
                        options.ApiKey = ApiKeyAuthentication.ApiKey;
                    });

            services.AddAuthorization();
            services.Configure<MvcOptions>(options =>
            {
                var policy = new AuthorizationPolicyBuilder(TestApiKeyAuthenticationDefaults.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });
        });
    }
}

[CollectionDefinition(CourseApiCollection.Name)]
public sealed class CourseApiCollection : ICollectionFixture<CourseApiFactory>
{
    public const string Name = "CourseApi collection";
}
