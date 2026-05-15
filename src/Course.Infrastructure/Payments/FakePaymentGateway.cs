using Course.Application.Abstractions;
using Course.Domain.Entities;

namespace Course.Infrastructure.Payments;

public sealed class FakePaymentGateway : IPaymentGateway
{
    private const decimal ApprovalLimit = 1000m;

    public Task<Payment> PayAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        var payment = Payment.Rejected(orderId, amount);
        return Task.FromResult(payment);

        //var payment = amount <= ApprovalLimit
        //    ? Payment.Approved(orderId, amount, $"SIM-{Guid.NewGuid():N}")
        //    : Payment.Rejected(orderId, amount);

        return Task.FromResult(payment);
    }
}
