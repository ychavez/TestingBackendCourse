using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.Domain.UnitTest;

public class ProductTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateProduct()
    {
        var id = Guid.NewGuid();
        var product = new Product(id, "Laptop", 1000m, 10);

        product.Id.Should().Be(id);
        product.Name.Should().Be("Laptop");
        product.Price.Should().Be(1000m);
        product.Stock.Should().Be(10);
    }

    [Fact]
    public void Constructor_WhenIdIsEmpty_ShouldThrow()
    {
        var act = () => new Product(Guid.Empty, "Laptop", 100m, 1);
        act.Should().Throw<DomainException>().WithMessage("*identificador*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNameIsEmpty_ShouldThrow(string name)
    {
        var act = () => new Product(Guid.NewGuid(), name, 100m, 1);
        act.Should().Throw<DomainException>().WithMessage("*nombre*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenPriceIsZeroOrNegative_ShouldThrow(decimal price)
    {
        var act = () => new Product(Guid.NewGuid(), "Laptop", price, 1);
        act.Should().Throw<DomainException>().WithMessage("*precio*");
    }

    [Fact]
    public void Constructor_WhenStockIsNegative_ShouldThrow()
    {
        var act = () => new Product(Guid.NewGuid(), "Laptop", 10m, -1);
        act.Should().Throw<DomainException>().WithMessage("*stock*");
    }

    [Fact]
    public void HasStock_ShouldReturnExpectedValue()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 5);

        product.HasStock(5).Should().BeTrue();
        product.HasStock(6).Should().BeFalse();
        product.HasStock(0).Should().BeFalse();
    }

    [Fact]
    public void DecreaseStock_WhenQuantityIsValid_ShouldDecrease()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 5);

        product.DecreaseStock(2);

        product.Stock.Should().Be(3);
    }

    [Fact]
    public void DecreaseStock_WhenInsufficient_ShouldThrow()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 1);

        var act = () => product.DecreaseStock(2);

        act.Should().Throw<InsufficientStockException>()
            .Which.Available.Should().Be(1);
    }

    [Fact]
    public void DecreaseStock_WhenQuantityIsZeroOrNegative_ShouldThrow()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 5);

        var act = () => product.DecreaseStock(0);

        act.Should().Throw<DomainException>().WithMessage("*cantidad*");
    }

    [Fact]
    public void IncreaseStock_WhenQuantityIsValid_ShouldIncrease()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 5);

        product.IncreaseStock(3);

        product.Stock.Should().Be(8);
    }

    [Fact]
    public void IncreaseStock_WhenQuantityIsZeroOrNegative_ShouldThrow()
    {
        var product = new Product(Guid.NewGuid(), "Laptop", 10m, 5);

        var act = () => product.IncreaseStock(0);

        act.Should().Throw<DomainException>().WithMessage("*cantidad*");
    }
}
