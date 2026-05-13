using Course.Application.Abstractions;
using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly InMemoryStore _store;

    public InMemoryCustomerRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
       
        _store.Customers.TryGetValue(id, out var customer);
        return Task.FromResult(customer);
    }
}
