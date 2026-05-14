using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.Api.IntegrationTest;

[Collection(CourseApiCollection.Name)]
public class ProductsEndpointsTests
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(CourseApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSeededProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products!.Should().HaveCount(3);
        products.Should().Contain(p => p.Id == DemoData.KeyboardId);
        products.Should().Contain(p => p.Id == DemoData.MouseId);
        products.Should().Contain(p => p.Id == DemoData.LaptopId);
    }
}
