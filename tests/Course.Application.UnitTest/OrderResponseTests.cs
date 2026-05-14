using Course.Application.Orders;
using Course.Domain.Entities;
using Course.Domain.Enums;
using FluentAssertions;

namespace Course.Application.UnitTest;

public class OrderResponseTests
{
    [Fact]
    public void FromOrder_ShouldMapAllFields()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);
        order.AddItem(product, 2);
        var payment = new Payment(Guid.NewGuid(), order.Id, 200m, PaymentStatus.Approved, "tx");
        order.RegisterPayment(payment);

        var response = OrderResponse.FromOrder(order);

        response.Id.Should().Be(order.Id);
        response.CustomerId.Should().Be(order.CustomerId);
        response.Total.Should().Be(200m);
        response.Status.Should().Be(OrderStatus.Paid);
        response.Items.Should().HaveCount(1);
        response.Payment.Should().NotBeNull();
        response.Payment!.Amount.Should().Be(200m);
    }

    [Fact]
    public void FromOrder_WhenNoPayment_ShouldMapNull()
    {
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);

        var response = OrderResponse.FromOrder(order);

        response.Payment.Should().BeNull();
        response.Items.Should().BeEmpty();
    }

    [Fact]
    public void ProductResponse_FromProduct_ShouldMap()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);

        var response = ProductResponse.FromProduct(product);

        response.Id.Should().Be(product.Id);
        response.Name.Should().Be("Laptop");
        response.Price.Should().Be(100m);
        response.Stock.Should().Be(5);
    }
}
