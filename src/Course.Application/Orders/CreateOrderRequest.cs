namespace Course.Application.Orders;

public sealed record CreateOrderRequest(Guid CustomerId, IReadOnlyCollection<CreateOrderItemRequest> Items);
