using System.Collections.Concurrent;
using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public sealed class InMemoryStore
{
    public InMemoryStore() : this(DemoData.Customers(), DemoData.Products()) { }
    


    public InMemoryStore(IEnumerable<Customer> customers, IEnumerable<Product> products)
    {
        Customers = new ConcurrentDictionary<Guid, Customer>(customers.ToDictionary(customer => customer.Id));
        Products = new ConcurrentDictionary<Guid, Product>(products.ToDictionary(product => product.Id));
        Orders = new ConcurrentDictionary<Guid, Order>();
        
    }

    public ConcurrentDictionary<Guid, Customer> Customers { get; }

    public ConcurrentDictionary<Guid, Product> Products { get; }

    public ConcurrentDictionary<Guid, Order> Orders { get; }
}
