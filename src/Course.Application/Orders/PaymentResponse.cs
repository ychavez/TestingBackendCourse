using Course.Domain.Enums;

namespace Course.Application.Orders;

public sealed record PaymentResponse(Guid Id, decimal Amount, PaymentStatus Status, string? TransactionId);
