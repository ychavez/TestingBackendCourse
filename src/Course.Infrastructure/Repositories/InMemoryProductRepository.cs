using Course.Application.Abstractions;
using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly InMemoryStore _store;

    public InMemoryProductRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default)
    {
        var products = _store.Products.Values.OrderBy(product => product.Name).ToArray();
        return Task.FromResult<IReadOnlyCollection<Product>>(products);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.Products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _store.Products[product.Id] = product;
        return Task.CompletedTask;
    }
}
