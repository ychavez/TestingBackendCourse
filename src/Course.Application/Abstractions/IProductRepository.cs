using Course.Domain.Entities;

namespace Course.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
