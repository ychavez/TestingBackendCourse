using System.Net;
using System.Net.Http.Json;
using Course.Application.Orders;
using Course.Domain.Enums;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.Api.IntegrationTest;

[Collection(CourseApiCollection.Name)]
public class OrdersEndpointsTests
{
    private readonly HttpClient _client;

    public OrdersEndpointsTests(CourseApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WhenValid_ShouldReturnCreatedAndPaid()
    {
        var request = new CreateOrderRequest(
            DemoData.CustomerId,
            new List<CreateOrderItemRequest> { new(DemoData.MouseId, 2) });

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        order.Should().NotBeNull();
        order!.CustomerId.Should().Be(DemoData.CustomerId);
        order.Total.Should().Be(90m);
        order.Status.Should().Be(OrderStatus.Paid);
        order.Payment.Should().NotBeNull();
        order.Payment!.Status.Should().Be(PaymentStatus.Approved);
    }

    [Fact]
    public async Task CreateOrder_WhenCustomerDoesNotExist_ShouldReturnNotFound()
    {
        var request = new CreateOrderRequest(
            Guid.NewGuid(),
            new List<CreateOrderItemRequest> { new(DemoData.MouseId, 1) });

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var request = new CreateOrderRequest(
            DemoData.CustomerId,
            new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WhenItemsEmpty_ShouldReturnBadRequest()
    {
        var request = new CreateOrderRequest(
            DemoData.CustomerId,
            new List<CreateOrderItemRequest>());

        var response = await _client.PostAsJsonAsync("/api/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrderById_WhenExists_ShouldReturnOrder()
    {
        var createRequest = new CreateOrderRequest(
            DemoData.CustomerId,
            new List<CreateOrderItemRequest> { new(DemoData.MouseId, 1) });
        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();

        var getResponse = await _client.GetAsync($"/api/orders/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();
        fetched!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetOrderById_WhenMissing_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CancelOrder_WhenPaid_ShouldReturnBadRequest()
    {
        var createRequest = new CreateOrderRequest(
            DemoData.CustomerId,
            new List<CreateOrderItemRequest> { new(DemoData.MouseId, 1) });
        var createResponse = await _client.PostAsJsonAsync("/api/orders", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();

        var cancelResponse = await _client.PostAsync($"/api/orders/{created!.Id}/cancel", content: null);

        cancelResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CancelOrder_WhenMissing_ShouldReturnNotFound()
    {
        var response = await _client.PostAsync($"/api/orders/{Guid.NewGuid()}/cancel", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
