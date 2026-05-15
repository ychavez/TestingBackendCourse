using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using FluentAssertions;

namespace Course.Api.IntegrationTest;

[Collection(CourseApiCollection.Name)]
public class ProductsEndpointsTests
{
    private readonly CourseApiFactory _factory;
    private readonly HttpClient _client;
    private readonly CourseApiTestDataFixture _testData;

    public ProductsEndpointsTests(CourseApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateAuthenticatedClient();
        _testData = factory.TestData;
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSeededProducts()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.Should().NotBeNull();
        products!.Should().HaveCount(3);
        products.Should().Contain(p => p.Id == _testData.KeyboardId);
        products.Should().Contain(p => p.Id == _testData.MouseId);
        products.Should().Contain(p => p.Id == _testData.MonitorId);
    }

    [Fact]
    public async Task GetProducts_WithoutApiKey_ShouldReturnUnauthorized()
    {
        var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
