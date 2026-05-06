using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Domain.Enums;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.E2ETests;

public sealed class OrderFlowE2ETests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrderFlowE2ETests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteOrderFlow_ShouldCreateReadAndRejectCancellationForPaidOrder()
    {
        var createRequest = new CreateOrderRequest(
            DemoData.CustomerId,
            [new CreateOrderItemRequest(DemoData.KeyboardId, 1)]);

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();
        createdOrder.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var storedOrder = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();
        storedOrder!.Status.Should().Be(OrderStatus.Paid);

        var cancelResponse = await _client.PostAsync($"/api/orders/{createdOrder.Id}/cancel", content: null);
        cancelResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RejectedPaymentFlow_ShouldAllowCancellation()
    {
        var createRequest = new CreateOrderRequest(
            DemoData.CustomerId,
            [new CreateOrderItemRequest(DemoData.LaptopId, 1)]);

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();
        createdOrder!.Status.Should().Be(OrderStatus.PaymentRejected);

        var cancelResponse = await _client.PostAsync($"/api/orders/{createdOrder.Id}/cancel", content: null);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var cancelledOrder = await cancelResponse.Content.ReadFromJsonAsync<OrderResponse>();
        cancelledOrder!.Status.Should().Be(OrderStatus.Cancelled);
    }
}
