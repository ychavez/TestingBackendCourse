using Course.Application.Abstractions;
using Course.Application.Orders;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;

    public ProductsController(IProductRepository products)
    {
        _products = products;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> List(CancellationToken cancellationToken)
    {
        var products = await _products.ListAsync(cancellationToken);
        return Ok(products.Select(ProductResponse.FromProduct).ToArray());
    }
}
