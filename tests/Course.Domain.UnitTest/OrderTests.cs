using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.Domain.UnitTest;

public class OrderTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateOrder()
    {
        var id = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var date = DateTimeOffset.UtcNow;

        var order = new Order(id, customerId, date);

        order.Id.Should().Be(id);
        order.CustomerId.Should().Be(customerId);
        order.CreatedAtUtc.Should().Be(date);
        order.Status.Should().Be(OrderStatus.Created);
        order.Items.Should().BeEmpty();
        order.Total.Should().Be(0);
        order.Payment.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhenIdEmpty_ShouldThrow()
    {
        var act = () => new Order(Guid.Empty, Guid.NewGuid(), DateTimeOffset.UtcNow);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WhenCustomerEmpty_ShouldThrow()
    {
        var act = () => new Order(Guid.NewGuid(), Guid.Empty, DateTimeOffset.UtcNow);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_ShouldAddItemAndUpdateTotal()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);

        order.AddItem(product, 2);

        order.Items.Should().HaveCount(1);
        order.Total.Should().Be(200m);
        product.Stock.Should().Be(3);
    }

    [Fact]
    public void RegisterPayment_WhenApproved_ShouldSetStatusPaid()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        var payment = new Payment(Guid.NewGuid(), order.Id, 100m, PaymentStatus.Approved, "tx");

        order.RegisterPayment(payment);

        order.Status.Should().Be(OrderStatus.Paid);
        order.Payment.Should().Be(payment);
    }

    [Fact]
    public void RegisterPayment_WhenRejected_ShouldSetStatusPaymentRejected()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        var payment = Payment.Rejected(order.Id, 100m);

        order.RegisterPayment(payment);

        order.Status.Should().Be(OrderStatus.PaymentRejected);
    }

    [Fact]
    public void RegisterPayment_WhenNoItems_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var payment = new Payment(Guid.NewGuid(), order.Id, 100m, PaymentStatus.Approved, "tx");

        var act = () => order.RegisterPayment(payment);

        act.Should().Throw<DomainException>().WithMessage("*items*");
    }

    [Fact]
    public void RegisterPayment_WhenOrderIdMismatch_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        var payment = new Payment(Guid.NewGuid(), Guid.NewGuid(), 100m, PaymentStatus.Approved, "tx");

        var act = () => order.RegisterPayment(payment);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RegisterPayment_WhenAmountMismatch_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        var payment = new Payment(Guid.NewGuid(), order.Id, 50m, PaymentStatus.Approved, "tx");

        var act = () => order.RegisterPayment(payment);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_WhenCreated_ShouldSetCancelled()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldNotThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        order.Cancel();

        var act = () => order.Cancel();

        act.Should().NotThrow();
    }

    [Fact]
    public void Cancel_WhenPaid_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 1);
        var payment = new Payment(Guid.NewGuid(), order.Id, 100m, PaymentStatus.Approved, "tx");
        order.RegisterPayment(payment);

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddItem_WhenNotCreatedStatus_ShouldThrow()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        order.Cancel();
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);

        var act = () => order.AddItem(product, 1);

        act.Should().Throw<DomainException>();
    }
}
