using Course.Application.Abstractions;
using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly InMemoryStore _store;

    public InMemoryOrderRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _store.Orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.Orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _store.Orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
