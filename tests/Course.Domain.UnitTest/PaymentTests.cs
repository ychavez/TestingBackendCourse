using Course.Domain.Entities;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.Domain.UnitTest;

public class PaymentTests
{
    [Fact]
    public void Constructor_WhenDataValid_ShouldCreatePayment()
    {
        var id = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var payment = new Payment(id, orderId, 100m, PaymentStatus.Approved, "TX-1");

        payment.Id.Should().Be(id);
        payment.OrderId.Should().Be(orderId);
        payment.Amount.Should().Be(100m);
        payment.Status.Should().Be(PaymentStatus.Approved);
        payment.TransactionId.Should().Be("TX-1");
    }

    [Fact]
    public void Constructor_WhenIdEmpty_ShouldThrow()
    {
        var act = () => new Payment(Guid.Empty, Guid.NewGuid(), 10m, PaymentStatus.Approved, "tx");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_WhenOrderIdEmpty_ShouldThrow()
    {
        var act = () => new Payment(Guid.NewGuid(), Guid.Empty, 10m, PaymentStatus.Approved, "tx");
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenAmountInvalid_ShouldThrow(decimal amount)
    {
        var act = () => new Payment(Guid.NewGuid(), Guid.NewGuid(), amount, PaymentStatus.Approved, "tx");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Approved_ShouldReturnApprovedPayment()
    {
        var orderId = Guid.NewGuid();

        var payment = Payment.Approved(orderId, 100m, "TX-1");

        payment.Status.Should().Be(PaymentStatus.Approved);
        payment.OrderId.Should().Be(orderId);
        payment.TransactionId.Should().Be("TX-1");
    }

    [Fact]
    public void Approved_WhenTransactionMissing_ShouldThrow()
    {
        var act = () => Payment.Approved(Guid.NewGuid(), 10m, "");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Rejected_ShouldReturnRejectedPayment()
    {
        var orderId = Guid.NewGuid();

        var payment = Payment.Rejected(orderId, 100m);

        payment.Status.Should().Be(PaymentStatus.Rejected);
        payment.TransactionId.Should().BeNull();
    }
}
