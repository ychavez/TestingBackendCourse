using Course.Domain.Entities;
using Course.Infrastructure.Repositories;

namespace Course.Api.IntegrationTest;

public sealed class CourseApiTestDataFixture
{
    public Guid CustomerId { get; } = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    public Guid KeyboardId { get; } = Guid.Parse("44444444-4444-4444-4444-444444444444");

    public Guid MouseId { get; } = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public Guid MonitorId { get; } = Guid.Parse("66666666-6666-6666-6666-666666666666");

    public InMemoryStore CreateStore()
    {
        return new InMemoryStore(Customers(), Products());
    }

    public IReadOnlyCollection<Customer> Customers()
    {
        return
        [
            new Customer(CustomerId, "Integration Test Customer", "integration.customer@example.com")
        ];
    }

    public IReadOnlyCollection<Product> Products()
    {
        return
        [
            new Product(KeyboardId, "Test Keyboard", 90m, 25),
            new Product(MouseId, "Test Mouse", 25m, 30),
            new Product(MonitorId, "Test Monitor", 300m, 10)
        ];
    }
}
