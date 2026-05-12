using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTests;

public sealed class CustomerTests
{
    [Fact]
    public void Constructor_WhenEmailIsInvalid_ShouldThrowDomainException()
    {
        var act = () => new Customer(Guid.NewGuid(), "Ana Backend", "invalid-email");

        act.Should().Throw<DomainException>()
            .WithMessage("El cliente requiere un correo valido.");
    }

    [Fact]
    public void Constructor_WhenDataIsValid_ShouldCreateCustomer()
    {
        var id = Guid.NewGuid();

        var customer = new Customer(id, "Ana Backend", "ana@example.com");

        customer.Id.Should().Be(id);
        customer.FullName.Should().Be("Ana Backend");
        customer.Email.Should().Be("ana@example.com");
    }
}
