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
}
