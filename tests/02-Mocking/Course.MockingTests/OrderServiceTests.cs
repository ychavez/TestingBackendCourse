using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Domain.Entities;
using Course.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Course.MockingTests;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenPaymentIsApproved_ShouldPersistPaidOrder()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Mouse", 50m, 10);
        var savedOrders = new List<Order>();

        var customers = new Mock<ICustomerRepository>();
        customers.Setup(repository => repository.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(customerId, "Ana Backend", "ana@example.com"));

        var products = new Mock<IProductRepository>();
        products.Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var orders = new Mock<IOrderRepository>();
        orders.Setup(repository => repository.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((order, _) => savedOrders.Add(order))
            .Returns(Task.CompletedTask);

        var payments = new Mock<IPaymentGateway>();
        payments.Setup(gateway => gateway.PayAsync(It.IsAny<Guid>(), 100m, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid orderId, decimal amount, CancellationToken _) => Payment.Approved(orderId, amount, "TX-001"));

        var service = new OrderService(customers.Object, products.Object, orders.Object, payments.Object);

        var response = await service.CreateAsync(new CreateOrderRequest(customerId, [new CreateOrderItemRequest(productId, 2)]));

        response.Status.Should().Be(OrderStatus.Paid);
        response.Total.Should().Be(100m);
        product.Stock.Should().Be(8);
        savedOrders.Should().ContainSingle(order => order.Id == response.Id);
        payments.Verify(gateway => gateway.PayAsync(response.Id, 100m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var customers = new Mock<ICustomerRepository>();
        customers.Setup(repository => repository.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(customerId, "Ana Backend", "ana@example.com"));

        var products = new Mock<IProductRepository>();
        products.Setup(repository => repository.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new OrderService(customers.Object, products.Object, Mock.Of<IOrderRepository>(), Mock.Of<IPaymentGateway>());

        var act = () => service.CreateAsync(new CreateOrderRequest(customerId, [new CreateOrderItemRequest(productId, 1)]));

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Producto no encontrado: {productId}.");
    }
}
