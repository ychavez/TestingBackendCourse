using Course.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Course.Api.IntegrationTest
{
    public sealed class CourseApiFactory : WebApplicationFactory<Program>
    {

        public CourseApiTestDataFixture TestData { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<InMemoryStore>();
                services.AddSingleton(_ => TestData.CreateStore());


            });

            base.ConfigureWebHost(builder);
        }


    }

    [CollectionDefinition(CourseApiCollection.Name)]
    public sealed class CourseApiCollection : ICollectionFixture<CourseApiFactory> 
    {
        public const string Name = "CourseApi collection";
    
    }
}
