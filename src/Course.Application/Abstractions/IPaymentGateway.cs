using Course.Domain.Entities;

namespace Course.Application.Abstractions;

public interface IPaymentGateway
{
    Task<Payment> PayAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}
