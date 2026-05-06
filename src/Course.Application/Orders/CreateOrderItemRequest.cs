namespace Course.Application.Orders;

public sealed record CreateOrderItemRequest(Guid ProductId, int Quantity);
