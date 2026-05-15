using Course.Application.Orders;
using Course.Infrastructure.Repositories;
using FluentAssertions;
using System.Net.Http.Json;

namespace Course.Api.IntegrationTest
{
    [Collection(CourseApiCollection.Name)]
    public class OrdersEndpointsTests
    {

        private readonly HttpClient _client;
        private readonly CourseApiTestDataFixture _fixture;

        public OrdersEndpointsTests(CourseApiFactory factory)
        {
            _client = factory.CreateClient();
            _fixture = factory.TestData;
        }

        [Fact]
        public async Task CreateOrder_WhenValid_ShouldReturnCreateAndApaid() 
        {
            //ARRANGE
            var request = new CreateOrderRequest(_fixture.CustomerId,
                new List<CreateOrderItemRequest> { new(_fixture.MouseId, 2) });


            //ACT
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            //ASSERT

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var order = await response.Content.ReadFromJsonAsync<OrderResponse>();

            order.Should().NotBeNull();
            order.CustomerId.Should().Be(_fixture.CustomerId);
            order.Total.Should().Be(50m);
            order.Payment.Should().NotBeNull();
            order.Payment.Status.Should().Be(Domain.Enums.PaymentStatus.Approved);
        }

        [Fact]
        public async Task CancelOrder_WhenMissing_ShouldReturnNotFound() 
        {
            // Arrange & Act
            var response = await _client.PostAsync($"/api/orders/{Guid.NewGuid()}/cancel", content: null);


            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

    }
}
