using Course.Domain.Exceptions;

namespace Course.Domain.Entities;

public sealed class OrderItem
{
    public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("El item requiere un producto.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("El item requiere el nombre del producto.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainException("El precio unitario debe ser mayor que cero.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid ProductId { get; }

    public string ProductName { get; }

    public decimal UnitPrice { get; }

    public int Quantity { get; }

    public decimal Total => UnitPrice * Quantity;

    public static OrderItem FromProduct(Product product, int quantity)
    {
        product.DecreaseStock(quantity);
        return new OrderItem(product.Id, product.Name, product.Price, quantity);
    }
}
