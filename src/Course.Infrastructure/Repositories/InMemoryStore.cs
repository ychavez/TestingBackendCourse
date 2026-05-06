using System.Collections.Concurrent;
using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public sealed class InMemoryStore
{
    public InMemoryStore()
    {
        Customers = new ConcurrentDictionary<Guid, Customer>(DemoData.Customers().ToDictionary(customer => customer.Id));
        Products = new ConcurrentDictionary<Guid, Product>(DemoData.Products().ToDictionary(product => product.Id));
        Orders = new ConcurrentDictionary<Guid, Order>();
    }

    public ConcurrentDictionary<Guid, Customer> Customers { get; }

    public ConcurrentDictionary<Guid, Product> Products { get; }

    public ConcurrentDictionary<Guid, Order> Orders { get; }
}
