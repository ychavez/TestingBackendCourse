using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using Course.Web.Clients;
using Course.Web.Models;
using FluentAssertions;

namespace Course.Web.UnitTest;

public class CourseApiClientTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _responder;
        public List<HttpRequestMessage> Requests { get; } = new();

        public StubHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responder)
        {
            _responder = responder;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(_responder(request, cancellationToken));
        }
    }

    private static CourseApiClient CreateClient(StubHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("http://localhost/") });

    [Fact]
    public async Task GetProductsAsync_WhenSuccess_ShouldReturnList()
    {
        var products = new[] { new ProductResponse(Guid.NewGuid(), "Laptop", 100m, 5) };
        var handler = new StubHandler((req, _) => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create<IReadOnlyList<ProductResponse>>(products)
        });
        var client = CreateClient(handler);

        var result = await client.GetProductsAsync();

        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be("/api/products");
    }

    [Fact]
    public async Task GetProductsAsync_WhenServerError_ShouldReturnFailureWithDetail()
    {
        var handler = new StubHandler((req, _) => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"detail\":\"problema\"}", Encoding.UTF8, "application/json")
        });
        var client = CreateClient(handler);

        var result = await client.GetProductsAsync();

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("problema");
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenSuccess_ShouldReturnOrder()
    {
        var orderResponse = new OrderResponse(
            Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow,
            OrderStatus.Paid, 100m, Array.Empty<OrderItemResponse>(), null);

        var handler = new StubHandler((req, _) => new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = JsonContent.Create(orderResponse)
        });
        var client = CreateClient(handler);

        var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });
        var result = await client.CreateOrderAsync(request);

        result.Success.Should().BeTrue();
        result.Value!.Id.Should().Be(orderResponse.Id);
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be("/api/orders");
    }

    [Fact]
    public async Task GetOrderAsync_WhenNotFound_ShouldReturnFailure()
    {
        var handler = new StubHandler((req, _) => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("")
        });
        var client = CreateClient(handler);

        var result = await client.GetOrderAsync(Guid.NewGuid());

        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CancelOrderAsync_WhenSuccess_ShouldReturnOrder()
    {
        var orderId = Guid.NewGuid();
        var orderResponse = new OrderResponse(
            orderId, Guid.NewGuid(), DateTimeOffset.UtcNow,
            OrderStatus.Cancelled, 100m, Array.Empty<OrderItemResponse>(), null);

        var handler = new StubHandler((req, _) => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(orderResponse)
        });
        var client = CreateClient(handler);

        var result = await client.CancelOrderAsync(orderId);

        result.Success.Should().BeTrue();
        result.Value!.Status.Should().Be(OrderStatus.Cancelled);
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].RequestUri!.AbsolutePath.Should().Be($"/api/orders/{orderId}/cancel");
    }

    [Fact]
    public async Task ExecuteAsync_WhenHttpRequestExceptionThrown_ShouldReturnFailure()
    {
        var handler = new StubHandler((req, _) => throw new HttpRequestException("boom"));
        var client = CreateClient(handler);

        var result = await client.GetProductsAsync();

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("boom");
    }
}
