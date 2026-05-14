using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace Course.Application.UnitTest;

public class OrderServiceTests
{
    private readonly Mock<ICustomerRepository> _customers = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IOrderRepository> _orders = new();
    private readonly Mock<IPaymentGateway> _payments = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(_customers.Object, _products.Object, _orders.Object, _payments.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_ShouldCreateOrder()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateOrderRequest(customerId,
            new List<CreateOrderItemRequest> { new(productId, 2) });

        var customer = new Customer(customerId, "Juan", "juan@example.com");
        var product = new Product(productId, "Laptop", 1000m, 10);

        _customers.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _products.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        _payments.Setup(x => x.PayAsync(It.IsAny<Guid>(), 2000m, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid orderId, decimal amount, CancellationToken _) =>
                new Payment(Guid.NewGuid(), orderId, amount, PaymentStatus.Approved, "tx"));

        var response = await _service.CreateAsync(request);

        response.Should().NotBeNull();
        response.CustomerId.Should().Be(customerId);
        response.Total.Should().Be(2000m);
        response.Status.Should().Be(OrderStatus.Paid);

        _orders.Verify(x => x.AddAsync(It.Is<Order>(o =>
            o.CustomerId == customerId &&
            o.Total == 2000m &&
            o.Status == OrderStatus.Paid &&
            o.Payment != null &&
            o.Payment.OrderId == o.Id), It.IsAny<CancellationToken>()), Times.Once);

        _products.Verify(x => x.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenItemsEmpty_ShouldThrowDomainException()
    {
        var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrderItemRequest>());

        var act = async () => await _service.CreateAsync(request);

        await act.Should().ThrowAsync<DomainException>();
        _orders.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCustomerNotFound_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var request = new CreateOrderRequest(customerId,
            new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });

        _customers.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        var act = async () => await _service.CreateAsync(request);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Cliente*");
        _orders.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenProductNotFound_ShouldThrow()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateOrderRequest(customerId,
            new List<CreateOrderItemRequest> { new(productId, 1) });

        _customers.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(customerId, "Juan", "juan@example.com"));
        _products.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var act = async () => await _service.CreateAsync(request);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("*Producto*");
        _orders.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnResponse()
    {
        var order = BuildPaidOrder();
        _orders.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var response = await _service.GetByIdAsync(order.Id);

        response.Should().NotBeNull();
        response!.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderMissing_ShouldReturnNull()
    {
        _orders.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var response = await _service.GetByIdAsync(Guid.NewGuid());

        response.Should().BeNull();
    }

    [Fact]
    public async Task CancelAsync_WhenOrderExists_ShouldCancelAndRestoreStock()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 2);

        _orders.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _products.Setup(x => x.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var response = await _service.CancelAsync(order.Id);

        response.Status.Should().Be(OrderStatus.Cancelled);
        product.Stock.Should().Be(5);
        _orders.Verify(x => x.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WhenOrderMissing_ShouldThrow()
    {
        _orders.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var act = async () => await _service.CancelAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    private static Order BuildPaidOrder()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        order.RegisterPayment(new Payment(Guid.NewGuid(), order.Id, 100m, PaymentStatus.Approved, "tx"));
        return order;
    }
}
