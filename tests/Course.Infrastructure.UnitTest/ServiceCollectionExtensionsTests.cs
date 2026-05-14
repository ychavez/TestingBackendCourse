using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Infrastructure.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Course.Infrastructure.UnitTest;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCourseInfrastructure_ShouldRegisterServices()
    {
        var services = new ServiceCollection();
        services.AddCourseInfrastructure();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        sp.GetService<ICustomerRepository>().Should().NotBeNull();
        sp.GetService<IProductRepository>().Should().NotBeNull();
        sp.GetService<IOrderRepository>().Should().NotBeNull();
        sp.GetService<IPaymentGateway>().Should().NotBeNull();
        sp.GetService<IOrderService>().Should().NotBeNull();
    }
}
