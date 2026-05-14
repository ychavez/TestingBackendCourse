using Course.Api.Controllers;
using Course.Application.Orders;
using Course.Domain.Enums;
using Course.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Course.Api.UnitTest;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _service = new();
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _controller = new OrdersController(_service.Object);
    }

    private static OrderResponse SampleResponse(Guid? id = null) => new(
        id ?? Guid.NewGuid(),
        Guid.NewGuid(),
        DateTimeOffset.UtcNow,
        OrderStatus.Paid,
        100m,
        Array.Empty<OrderItemResponse>(),
        null);

    [Fact]
    public async Task Create_WhenValid_ShouldReturnCreated()
    {
        var response = SampleResponse();
        _service.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });
        var result = await _controller.Create(request, CancellationToken.None);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.Value.Should().Be(response);
        created.ActionName.Should().Be(nameof(OrdersController.GetById));
    }

    [Fact]
    public async Task Create_WhenKeyNotFound_ShouldReturnNotFound()
    {
        _service.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("missing"));

        var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });
        var result = await _controller.Create(request, CancellationToken.None);

        var notFound = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var problem = notFound.Value.Should().BeOfType<ProblemDetails>().Subject;
        problem.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Create_WhenDomainException_ShouldReturnBadRequest()
    {
        _service.Setup(x => x.CreateAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DomainException("invalid"));

        var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrderItemRequest> { new(Guid.NewGuid(), 1) });
        var result = await _controller.Create(request, CancellationToken.None);

        var bad = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var problem = bad.Value.Should().BeOfType<ProblemDetails>().Subject;
        problem.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetById_WhenExists_ShouldReturnOk()
    {
        var response = SampleResponse();
        _service.Setup(x => x.GetByIdAsync(response.Id, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.GetById(response.Id, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(response);
    }

    [Fact]
    public async Task GetById_WhenMissing_ShouldReturnNotFound()
    {
        _service.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

        var result = await _controller.GetById(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Cancel_WhenValid_ShouldReturnOk()
    {
        var response = SampleResponse();
        _service.Setup(x => x.CancelAsync(response.Id, It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _controller.Cancel(response.Id, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(response);
    }

    [Fact]
    public async Task Cancel_WhenKeyNotFound_ShouldReturnNotFound()
    {
        _service.Setup(x => x.CancelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("missing"));

        var result = await _controller.Cancel(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Cancel_WhenDomainException_ShouldReturnBadRequest()
    {
        _service.Setup(x => x.CancelAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DomainException("invalid"));

        var result = await _controller.Cancel(Guid.NewGuid(), CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
