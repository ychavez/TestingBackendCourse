namespace Course.Application.Orders;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrderResponse> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
