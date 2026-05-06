using Course.Domain.Exceptions;

namespace Course.Domain.Entities;

public sealed class Product
{
    public Product(Guid id, string name, decimal price, int stock)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El producto requiere un identificador.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El producto requiere un nombre.");
        }

        if (price <= 0)
        {
            throw new DomainException("El precio debe ser mayor que cero.");
        }

        if (stock < 0)
        {
            throw new DomainException("El stock no puede ser negativo.");
        }

        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool HasStock(int quantity) => quantity > 0 && Stock >= quantity;

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        if (!HasStock(quantity))
        {
            throw new InsufficientStockException(Name, quantity, Stock);
        }

        Stock -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("La cantidad debe ser mayor que cero.");
        }

        Stock += quantity;
    }
}
