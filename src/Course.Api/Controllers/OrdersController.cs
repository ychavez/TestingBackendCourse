using Course.Application.Orders;
using Course.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orders.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ToProblemDetails(exception.Message, StatusCodes.Status404NotFound));
        }
        catch (DomainException exception)
        {
            return BadRequest(ToProblemDetails(exception.Message, StatusCodes.Status400BadRequest));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orders.CancelAsync(id, cancellationToken);
            return Ok(order);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(ToProblemDetails(exception.Message, StatusCodes.Status404NotFound));
        }
        catch (DomainException exception)
        {
            return BadRequest(ToProblemDetails(exception.Message, StatusCodes.Status400BadRequest));
        }
    }

    private static ProblemDetails ToProblemDetails(string detail, int statusCode)
    {
        return new ProblemDetails
        {
            Title = statusCode == StatusCodes.Status404NotFound ? "Recurso no encontrado" : "Regla de negocio invalida",
            Detail = detail,
            Status = statusCode
        };
    }
}
