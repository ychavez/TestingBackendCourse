using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Infrastructure.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.UnitTest
{
    public class OrderTests
    {
        private readonly Mock<ICustomerRepository> _customers = new();
        private readonly Mock<IProductRepository> _products = new();
        private readonly Mock<IOrderRepository> _orders = new();
        private readonly Mock<IPaymentGateway> _payments = new();

        private readonly OrderService _service;
        public OrderTests()
        {
            _service = new OrderService
                (_customers.Object,
                _products.Object,
                _orders.Object,
                _payments.Object); 
        }


        [Fact]
        public async Task Create_WhenOrderIsValid_ShouldCreateOrder()
        {

            //Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var itemsRequest = new List<CreateOrderItemRequest>();

            itemsRequest.Add(new CreateOrderItemRequest(productId, 2));

            var request = new CreateOrderRequest(customerId, itemsRequest);

            var customer = new Customer(customerId, "Juan perez", "Juanperez@gmail.com");
            var product = new Product(productId, "Laptop", 1000, 10);
            var payment = new Payment(Guid.NewGuid(),
                Guid.NewGuid(), 2000, 
                PaymentStatus.Approved,"");


            // Act 




        }

    }
}
