using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Domain.Enums;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.IntegrationTests;

public sealed class OrderApiIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrderApiIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostOrders_WhenRequestIsValid_ShouldReturnCreatedOrder()
    {
        var request = new CreateOrderRequest(
            DemoData.CustomerId,
            [new CreateOrderItemRequest(DemoData.MouseId, 2)]);

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        order.Should().NotBeNull();
        order!.Status.Should().Be(OrderStatus.Paid);
        order.Total.Should().Be(90m);
        order.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task PostOrders_WhenStockIsInsufficient_ShouldReturnBadRequest()
    {
        var request = new CreateOrderRequest(
            DemoData.CustomerId,
            [new CreateOrderItemRequest(DemoData.LaptopId, 99)]);

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
