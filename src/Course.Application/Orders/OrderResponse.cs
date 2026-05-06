using Course.Domain.Entities;
using Course.Domain.Enums;

namespace Course.Application.Orders;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    DateTimeOffset CreatedAtUtc,
    OrderStatus Status,
    decimal Total,
    IReadOnlyCollection<OrderItemResponse> Items,
    PaymentResponse? Payment)
{
    public static OrderResponse FromOrder(Order order)
    {
        var payment = order.Payment is null
            ? null
            : new PaymentResponse(order.Payment.Id, order.Payment.Amount, order.Payment.Status, order.Payment.TransactionId);

        var items = order.Items
            .Select(item => new OrderItemResponse(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity, item.Total))
            .ToArray();

        return new OrderResponse(order.Id, order.CustomerId, order.CreatedAtUtc, order.Status, order.Total, items, payment);
    }
}
