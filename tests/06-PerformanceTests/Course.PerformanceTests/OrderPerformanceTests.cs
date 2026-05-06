using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using NBomber.CSharp;

namespace Course.PerformanceTests;

public sealed class OrderPerformanceTests
{
    [Fact(Skip = "Prueba de performance didactica. Ejecutar manualmente para no ralentizar CI.")]
    public void CreateOrder_ShouldHandleSmallLoad()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureLogging(logging => logging.ClearProviders());
            });
        using var client = factory.CreateClient();

        var scenario = Scenario.Create("create_order", async context =>
        {
            var request = new CreateOrderRequest(
                DemoData.CustomerId,
                [new CreateOrderItemRequest(DemoData.MouseId, 1)]);

            var response = await client.PostAsJsonAsync("/api/orders", request);

            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail(statusCode: response.StatusCode.ToString());
        })
        .WithLoadSimulations(
            Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(10)));

        NBomberRunner.RegisterScenarios(scenario).Run();
    }
}
