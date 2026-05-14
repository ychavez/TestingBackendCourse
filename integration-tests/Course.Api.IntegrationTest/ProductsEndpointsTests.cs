using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.Api.IntegrationTest;

public class ProductsEndpointsTests
{
    [Fact]
    public async Task GetProducts_ShouldReturnSeededProducts()
    {
        using var factory = new CourseApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products!.Should().HaveCount(3);
        products.Should().Contain(p => p.Id == DemoData.KeyboardId);
        products.Should().Contain(p => p.Id == DemoData.MouseId);
        products.Should().Contain(p => p.Id == DemoData.LaptopId);
    }
}
