namespace Course.Web.Models;

public sealed record CreateOrderRequest(Guid CustomerId, IReadOnlyCollection<CreateOrderItemRequest> Items);

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity);

public sealed record ProductResponse(Guid Id, string Name, decimal Price, int Stock);

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    DateTimeOffset CreatedAtUtc,
    OrderStatus Status,
    decimal Total,
    IReadOnlyCollection<OrderItemResponse> Items,
    PaymentResponse? Payment);

public sealed record OrderItemResponse(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal Total);

public sealed record PaymentResponse(Guid Id, decimal Amount, PaymentStatus Status, string? TransactionId);

public enum OrderStatus
{
    Created = 1,
    Paid = 2,
    PaymentRejected = 3,
    Cancelled = 4
}

public enum PaymentStatus
{
    Approved = 1,
    Rejected = 2
}
