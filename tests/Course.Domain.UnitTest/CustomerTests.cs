using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.Domain.UnitTest;

public class CustomerTests
{
    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateCustomer()
    {
        var id = Guid.NewGuid();

        var customer = new Customer(id, "Juan Perez", "juan@example.com");

        customer.Id.Should().Be(id);
        customer.FullName.Should().Be("Juan Perez");
        customer.Email.Should().Be("juan@example.com");
    }

    [Fact]
    public void Constructor_WhenIdIsEmpty_ShouldThrow()
    {
        var act = () => new Customer(Guid.Empty, "Juan", "juan@example.com");

        act.Should().Throw<DomainException>()
            .WithMessage("*identificador*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenFullNameIsEmpty_ShouldThrow(string name)
    {
        var act = () => new Customer(Guid.NewGuid(), name, "juan@example.com");

        act.Should().Throw<DomainException>()
            .WithMessage("*nombre*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("noatsign")]
    public void Constructor_WhenEmailIsInvalid_ShouldThrow(string email)
    {
        var act = () => new Customer(Guid.NewGuid(), "Juan", email);

        act.Should().Throw<DomainException>()
            .WithMessage("*correo*");
    }
}
