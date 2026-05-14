using Course.Domain.Entities;
using Course.Infrastructure.Repositories;
using FluentAssertions;

namespace Course.Infrastructure.UnitTest;

public class InMemoryRepositoryTests
{
    [Fact]
    public async Task CustomerRepository_GetByIdAsync_ShouldReturnSeededCustomer()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryCustomerRepository(store);

        var customer = await repo.GetByIdAsync(DemoData.CustomerId);

        customer.Should().NotBeNull();
        customer!.Id.Should().Be(DemoData.CustomerId);
    }

    [Fact]
    public async Task CustomerRepository_GetByIdAsync_WhenMissing_ShouldReturnNull()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryCustomerRepository(store);

        var customer = await repo.GetByIdAsync(Guid.NewGuid());

        customer.Should().BeNull();
    }

    [Fact]
    public async Task ProductRepository_ListAsync_ShouldReturnSeededProductsOrderedByName()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryProductRepository(store);

        var products = await repo.ListAsync();

        products.Should().HaveCount(3);
        products.Select(p => p.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task ProductRepository_GetById_AndUpdate_ShouldPersistChanges()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryProductRepository(store);

        var product = await repo.GetByIdAsync(DemoData.KeyboardId);
        product.Should().NotBeNull();

        product!.DecreaseStock(1);
        await repo.UpdateAsync(product);

        var updated = await repo.GetByIdAsync(DemoData.KeyboardId);
        updated!.Stock.Should().Be(product.Stock);
    }

    [Fact]
    public async Task OrderRepository_AddGetUpdate_ShouldWork()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryOrderRepository(store);
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);

        await repo.AddAsync(order);
        var fetched = await repo.GetByIdAsync(order.Id);
        fetched.Should().Be(order);

        await repo.UpdateAsync(order);
        var refetched = await repo.GetByIdAsync(order.Id);
        refetched.Should().Be(order);
    }

    [Fact]
    public async Task OrderRepository_GetByIdAsync_WhenMissing_ShouldReturnNull()
    {
        var store = new InMemoryStore();
        var repo = new InMemoryOrderRepository(store);

        var fetched = await repo.GetByIdAsync(Guid.NewGuid());

        fetched.Should().BeNull();
    }
}
