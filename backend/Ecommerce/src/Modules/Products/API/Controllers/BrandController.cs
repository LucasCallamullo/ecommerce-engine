namespace Ecommerce.Products.API.Controllers;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;
using Ecommerce.Shared.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

/// <summary>REST API Controller managing catalog brand operations.</summary>
[Route("api/v1/brands")]
public class BrandsController(IBrandService brandService) : ApiControllerBase
{
    private readonly IBrandService _brandService = brandService;

    //* =====================================================================
    //* GET METHODS
    //* =====================================================================

    /// <summary>Retrieves all active catalog brands.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var brands = await _brandService.GetAllAsync(cancellationToken);
        return Ok(brands);
    }

    /// <summary>Retrieves basic brand information by its ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BrandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var brand = await _brandService.GetByIdAsync(id, cancellationToken);
        return Ok(brand);
    }

    /// <summary>Retrieves complete metadata for a brand by its ID.</summary>
    [HttpGet("{id:int}/detail")]
    [ProducesResponseType(typeof(BrandDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdDetail(int id, CancellationToken cancellationToken)
    {
        var brandDetail = await _brandService.GetByIdDetailAsync(id, cancellationToken);
        return Ok(brandDetail);
    }

    //* =====================================================================
    //*     POST / PUT / DELETE METHODS
    //* =====================================================================

    /// <summary>Creates a new brand entry in the catalog.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BrandResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] BrandCreateRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _brandService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Updates an existing brand entry by its ID.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BrandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id, 
        [FromBody] BrandUpdateRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _brandService.UpdateAsync(id, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>Performs a logical soft deletion on a brand by its ID.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _brandService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}