using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTests;

public sealed class OrderTests
{
    [Fact]
    public void Constructor_WhenCustomerIdIsEmpty_ShouldThrowDomainException()
    {
        var act = () => new Order(Guid.NewGuid(), Guid.Empty, DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("El pedido requiere un cliente.");
    }

    [Fact]
    public void CalculateTotal_ShouldSumAllItems()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var keyboard = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);
        var mouse = new Product(Guid.NewGuid(), "Mouse", 50m, 10);

        order.AddItem(keyboard, 2);
        order.AddItem(mouse, 1);

        order.CalculateTotal().Should().Be(250m);
    }

    [Fact]
    public void RegisterPayment_WhenApproved_ShouldMarkOrderAsPaid()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);
        order.AddItem(product, 1);

        order.RegisterPayment(Payment.Approved(order.Id, order.Total, "TX-001"));

        order.Status.Should().Be(OrderStatus.Paid);
        order.Payment.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenOrderIsPaid_ShouldThrowDomainException()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);
        order.AddItem(product, 1);
        order.RegisterPayment(Payment.Approved(order.Id, order.Total, "TX-001"));

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>()
            .WithMessage("No se puede cancelar un pedido pagado.");
    }

    [Fact]
    public void RegisterPayment_WhenPaymentIsRejected_ShouldMarkOrderAsPaymentRejected()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 1200m, 10);
        order.AddItem(product, 1);

        order.RegisterPayment(Payment.Rejected(order.Id, order.Total));

        order.Status.Should().Be(OrderStatus.PaymentRejected);
        order.Payment.Should().NotBeNull();
    }

    [Fact]
    public void RegisterPayment_WhenAmountDoesNotMatchTotal_ShouldThrowDomainException()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);
        order.AddItem(product, 1);

        var act = () => order.RegisterPayment(Payment.Approved(order.Id, 99m, "TX-001"));

        act.Should().Throw<DomainException>()
            .WithMessage("El monto del pago no coincide con el total.");
    }

    [Fact]
    public void AddItem_WhenOrderIsNotCreated_ShouldThrowDomainException()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);
        order.AddItem(product, 1);
        order.RegisterPayment(Payment.Rejected(order.Id, order.Total));

        var act = () => order.AddItem(product, 1);

        act.Should().Throw<DomainException>()
            .WithMessage("Solo se pueden modificar pedidos en estado creado.");
    }

    [Fact]
    public void Cancel_WhenOrderHasRejectedPayment_ShouldMarkOrderAsCancelled()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 1200m, 10);
        order.AddItem(product, 1);
        order.RegisterPayment(Payment.Rejected(order.Id, order.Total));

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
