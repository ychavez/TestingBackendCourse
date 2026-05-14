using Course.Domain.Enums;
using Course.Domain.Exceptions;

namespace Course.Domain.Entities;

public sealed class Payment
{
    public Payment(Guid id, Guid orderId, decimal amount, PaymentStatus status, string? transactionId)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El pago requiere un identificador.");
        }

        if (orderId == Guid.Empty)
        {
            throw new DomainException("El pago requiere un pedido.");
        }

        if (amount <= 0)
        {
            throw new DomainException("El monto del pago debe ser mayor que cero.");
        }

        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = status;
        TransactionId = transactionId;
    }

    public Guid Id { get; }

    public Guid OrderId { get; }

    public decimal Amount { get; }

    public PaymentStatus Status { get; }

    public string? TransactionId { get; }

    public static Payment Approved(Guid orderId, decimal amount, string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new DomainException("El pago aprobado requiere una transaccion.");
        }

        return new Payment(Guid.NewGuid(), orderId, amount, PaymentStatus.Approved, transactionId);
    }

    public static Payment Rejected(Guid orderId, decimal amount)
    {
        return new Payment(Guid.NewGuid(), orderId, amount, PaymentStatus.Rejected, null);
    }
}
