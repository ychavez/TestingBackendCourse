using Course.Application.Orders;
using Course.Infrastructure.Repositories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Course.Api.IntegrationTest
{
    [Collection(CourseApiCollection.Name)]
    public class OrdersEndpointsTests
    {

        private readonly HttpClient _client;

        public OrdersEndpointsTests(CourseApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateOrder_WhenValid_ShouldReturnCreateAndApaid() 
        {
            //ARRANGE
            var request = new CreateOrderRequest(DemoData.CustomerId,
                new List<CreateOrderItemRequest> { new(DemoData.MouseId, 2) });


            //ACT
            var response = await _client.PostAsJsonAsync("/api/orders", request);

            //ASSERT

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            var order = await response.Content.ReadFromJsonAsync<OrderResponse>();

            order.Should().NotBeNull();
            order.CustomerId.Should().Be(DemoData.CustomerId);
            order.Total.Should().Be(90m);
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
