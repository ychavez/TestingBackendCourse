using Course.Domain.Enums;
using Course.Infrastructure.Payments;
using FluentAssertions;

namespace Course.Infrastructure.UnitTest;

public class FakePaymentGatewayTests
{
    [Fact]
    public async Task PayAsync_WhenAmountAtOrBelowLimit_ShouldApprove()
    {
        var gateway = new FakePaymentGateway();
        var orderId = Guid.NewGuid();

        var payment = await gateway.PayAsync(orderId, 1000m);

        payment.Status.Should().Be(PaymentStatus.Approved);
        payment.OrderId.Should().Be(orderId);
        payment.TransactionId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task PayAsync_WhenAmountAboveLimit_ShouldReject()
    {
        var gateway = new FakePaymentGateway();
        var orderId = Guid.NewGuid();

        var payment = await gateway.PayAsync(orderId, 1500m);

        payment.Status.Should().Be(PaymentStatus.Rejected);
        payment.OrderId.Should().Be(orderId);
        payment.TransactionId.Should().BeNull();
    }
}
