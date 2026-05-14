using Course.Api.Controllers;
using Course.Application.Abstractions;
using Course.Application.Orders;
using Course.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Course.Api.UnitTest;

public class ProductsControllerTests
{
    [Fact]
    public async Task List_ShouldReturnProducts()
    {
        var repo = new Mock<IProductRepository>();
        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Keyboard", 50m, 5),
            new(Guid.NewGuid(), "Mouse", 25m, 10)
        };
        repo.Setup(x => x.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(products);

        var controller = new ProductsController(repo.Object);

        var result = await controller.List(CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var responses = ok.Value.Should().BeAssignableTo<IReadOnlyCollection<ProductResponse>>().Subject;
        responses.Should().HaveCount(2);
    }
}
