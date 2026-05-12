using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTests;

public sealed class PaymentTests
{
    [Fact]
    public void Rejected_WhenAmountIsZero_ShouldThrowDomainException()
    {
        var act = () => Payment.Rejected(Guid.NewGuid(), 0m);

        act.Should().Throw<DomainException>()
            .WithMessage("El monto del pago debe ser mayor que cero.");
    }

    [Fact]
    public void Approved_WhenTransactionIdIsEmpty_ShouldThrowDomainException()
    {
        var act = () => Payment.Approved(Guid.NewGuid(), 100m, string.Empty);

        act.Should().Throw<DomainException>()
            .WithMessage("El pago aprobado requiere una transaccion.");
    }

    [Fact]
    public void Rejected_ShouldCreateRejectedPaymentWithoutTransactionId()
    {
        var orderId = Guid.NewGuid();

        var payment = Payment.Rejected(orderId, 250m);

        payment.OrderId.Should().Be(orderId);
        payment.Amount.Should().Be(250m);
        payment.Status.Should().Be(PaymentStatus.Rejected);
        payment.TransactionId.Should().BeNull();
    }
}
