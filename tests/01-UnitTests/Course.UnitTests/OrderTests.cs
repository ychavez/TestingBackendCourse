using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTests;

public sealed class OrderTests
{
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
}
