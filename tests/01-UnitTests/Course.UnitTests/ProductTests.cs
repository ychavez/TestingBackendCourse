using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTests;

public sealed class ProductTests
{
    [Fact]
    public void DecreaseStock_WhenQuantityIsAvailable_ShouldUpdateStock()
    {
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 10);

        product.DecreaseStock(3);

        product.Stock.Should().Be(7);
    }

    [Fact]
    public void DecreaseStock_WhenQuantityExceedsStock_ShouldThrowDomainException()
    {
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 2);

        var act = () => product.DecreaseStock(3);

        act.Should().Throw<InsufficientStockException>()
            .WithMessage("*Solicitado: 3, disponible: 2*");
    }

    [Fact]
    public void IncreaseStock_WhenQuantityIsValid_ShouldRestoreInventory()
    {
        var product = new Product(Guid.NewGuid(), "Keyboard", 100m, 2);

        product.IncreaseStock(4);

        product.Stock.Should().Be(6);
    }

    [Fact]
    public void Constructor_WhenPriceIsZero_ShouldThrowDomainException()
    {
        var act = () => new Product(Guid.NewGuid(), "Keyboard", 0m, 2);

        act.Should().Throw<DomainException>()
            .WithMessage("El precio debe ser mayor que cero.");
    }
}
