using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.Domain.UnitTest;

public class OrderItemTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateItem()
    {
        var productId = Guid.NewGuid();
        var item = new OrderItem(productId, "Laptop", 100m, 2);

        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be("Laptop");
        item.UnitPrice.Should().Be(100m);
        item.Quantity.Should().Be(2);
        item.Total.Should().Be(200m);
    }

    [Fact]
    public void Constructor_WhenProductIdIsEmpty_ShouldThrow()
    {
        var act = () => new OrderItem(Guid.Empty, "Laptop", 100m, 1);
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Constructor_WhenProductNameIsEmpty_ShouldThrow(string name)
    {
        var act = () => new OrderItem(Guid.NewGuid(), name, 100m, 1);
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WhenUnitPriceInvalid_ShouldThrow(decimal price)
    {
        var act = () => new OrderItem(Guid.NewGuid(), "Laptop", price, 1);
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenQuantityInvalid_ShouldThrow(int qty)
    {
        var act = () => new OrderItem(Guid.NewGuid(), "Laptop", 10m, qty);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void FromProduct_ShouldCreateItemAndDecreaseStock()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 100m, 5);

        var item = OrderItem.FromProduct(product, 2);

        item.ProductId.Should().Be(product.Id);
        item.Quantity.Should().Be(2);
        product.Stock.Should().Be(3);
    }
}
