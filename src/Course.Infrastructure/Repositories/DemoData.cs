using Course.Domain.Entities;

namespace Course.Infrastructure.Repositories;

public static class DemoData
{
    public static readonly Guid CustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    public static readonly Guid KeyboardId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid MouseId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid LaptopId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static IReadOnlyCollection<Customer> Customers()
    {
        return
        [
            new Customer(CustomerId, "Ana Backend", "ana.backend@example.com")
        ];
    }

    public static IReadOnlyCollection<Product> Products()
    {
        return
        [
            new Product(KeyboardId, "Mechanical Keyboard", 85m, 20),
            new Product(MouseId, "Ergonomic Mouse", 45m, 35),
            new Product(LaptopId, "Developer Laptop", 1200m, 5)
        ];
    }
}
