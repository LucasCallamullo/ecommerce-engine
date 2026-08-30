using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Products.API.Controllers;

/// REST API Controller managing product operations.
/// Injects the application service contract via Primary Constructor or standard DI.
[ApiController]
[Route("api/[controller]")]
public class VariantsController(IVariantService variantService) : ControllerBase
{
    private readonly IVariantService _variantService = variantService;

    // =====================================================================
    //        GET METHODS
    // =====================================================================

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductVariantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _variantService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductVariantResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var products = await _variantService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    // =====================================================================
    //        POST / UPDATE / DELETE METHODS
    // =====================================================================

    [HttpPost("/api/products/{productId:int}/variants")]
    [ProducesResponseType(typeof(ProductVariantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromRoute] int productId,
        [FromBody] ProductCreateVariantRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _variantService.CreateAsync(productId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("/api/products/{productId:int}/variants/{id:int}")]
    [ProducesResponseType(typeof(ProductVariantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] int productId,
        [FromRoute] int id,
        [FromBody] ProductVariantUpdateRequest request, 
        CancellationToken cancellationToken)
    {
        var product = await _variantService.UpdateAsync(
            productId, 
            id,
            request,
            cancellationToken);
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        bool qsy = await _variantService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}