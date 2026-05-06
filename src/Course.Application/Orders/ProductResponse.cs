using Course.Domain.Entities;

namespace Course.Application.Orders;

public sealed record ProductResponse(Guid Id, string Name, decimal Price, int Stock)
{
    public static ProductResponse FromProduct(Product product)
    {
        return new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
    }
}
