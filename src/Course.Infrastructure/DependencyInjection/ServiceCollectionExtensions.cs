using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Infrastructure.Payments;
using Course.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Course.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStore>();
        services.AddScoped<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddScoped<IProductRepository, InMemoryProductRepository>();
        services.AddScoped<IOrderRepository, InMemoryOrderRepository>();
        services.AddScoped<IPaymentGateway, FakePaymentGateway>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
