using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Products.API.Controllers;

/// <summary>REST API Controller managing product variant endpoints.</summary>
[ApiController]
[Route("api/[controller]")]
public class VariantsController(IVariantService variantService) : ControllerBase
{
    private readonly IVariantService _variantService = variantService;

    //? =====================================================================
    //?         GET METHODS
    //? =====================================================================

    /// <summary>Retrieves a specific product variant by its identifier.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductVariantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var variant = await _variantService.GetByIdAsync(id, cancellationToken);
        return Ok(variant);
    }

    /// <summary>Retrieves all active product variants across the catalog.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductVariantResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var variants = await _variantService.GetAllAsync(cancellationToken);
        return Ok(variants);
    }

    /// <summary>Retrieves all active variants associated with a specific product.</summary>
    [HttpGet("/api/products/{productId:int}/variants")]
    [ProducesResponseType(typeof(IEnumerable<ProductVariantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariants(int productId, CancellationToken cancellationToken)
    {
        var variants = await _variantService
            .GetVariantsByProductId(productId, cancellationToken);
        return Ok(variants);
    }

    //? =====================================================================
    //?        METHODS --> POST / UPDATE / DELETE 
    //? =====================================================================

    /// <summary>Creates a new variant under an existing product.</summary>
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

    /// <summary>Updates an existing product variant by its identifier.</summary>
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
        var variant = await _variantService.UpdateAsync(
            productId, 
            id,
            request,
            cancellationToken);
        return Ok(variant);
    }

    /// <summary>Performs logical soft deletion on a product variant.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _variantService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}