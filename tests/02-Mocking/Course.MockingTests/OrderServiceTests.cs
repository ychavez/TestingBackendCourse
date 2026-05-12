using Course.Application.Abstractions;
using Course.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Course.MockingTests;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CustomerRepositoryMock_WhenCustomerExists_ShouldReturnConfiguredCustomer()
    {
        var customerId = Guid.NewGuid();
        var customers = new Mock<ICustomerRepository>();
        customers.Setup(repository => repository.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(customerId, "Ana Backend", "ana@example.com"));

        var customer = await customers.Object.GetByIdAsync(customerId);

        customer.Should().NotBeNull();
        customer!.Id.Should().Be(customerId);
        customer.Email.Should().Be("ana@example.com");
    }

    [Fact]
    public async Task ProductRepositoryMock_WhenProductIsUpdated_ShouldVerifyInteraction()
    {
        var product = new Product(Guid.NewGuid(), "Mouse", 50m, 10);
        var products = new Mock<IProductRepository>();
        products.Setup(repository => repository.UpdateAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        product.DecreaseStock(2);
        await products.Object.UpdateAsync(product);

        product.Stock.Should().Be(8);
        products.Verify(repository => repository.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }
}
