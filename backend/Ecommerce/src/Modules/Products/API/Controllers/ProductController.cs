namespace Ecommerce.Products.API.Controllers;

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Ecommerce.Products.Application.DTOs.Request;
using Ecommerce.Products.Application.DTOs.Response;
using Ecommerce.Products.Application.Interfaces;

using Ecommerce.Shared.API;
using Ecommerce.Shared.Auth.Constants;
using Ecommerce.Shared.Exceptions;


/// <summary>
/// REST API Controller managing product catalog operations.
/// </summary>
[Route("api/v1/products")]
public class ProductsController(IProductService productService) : ApiControllerBase
{
    private readonly IProductService _productService = productService;

    /// <summary>Imports multiple products asynchronously from a CSV file payload.</summary>
    [HttpPost("import-csv")]
    [Authorize(Roles = UserRoles.Admin)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportCsv(
        IFormFile file, 
        [FromServices] IProductImportService importService, 
        CancellationToken cancellationToken)
    {
        // HTTP-level validation
        if (file == null || file.Length == 0)
            throw new AppException("Please upload a valid, non-empty Excel file.", HttpStatusCode.BadRequest);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (extension != ".xlsx")
            throw new AppException(
                "Invalid file format. Only Microsoft Excel (.xlsx) files are supported.", 
                HttpStatusCode.BadRequest);

        // Pass clean Stream to the Application Service
        using var stream = file.OpenReadStream();
        var result = await importService.ImportFromExcelAsync(stream, cancellationToken);
        
        return Ok(result);
    }

    //? =====================================================================
    //?        GET METHODS
    //? =====================================================================

    /// <summary>Retrieves a basic master product representation by its ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    /// <summary>Retrieves a detailed product including categories, brands, images, and variants.</summary>
    [HttpGet("{id:int}/detail")]
    [ProducesResponseType(typeof(ProductDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdDetail(int id, CancellationToken cancellationToken)
    {
        var productDetail = await _productService.GetByIdDetailAsync(id, cancellationToken);
        return Ok(productDetail);
    }

    /// <summary>Retrieves all active non-deleted products in the catalog.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    //? =====================================================================
    //?       POST / UPDATE / DELETE METHODS
    //? =====================================================================

    /// <summary>Creates a new product entity with product/s variant entity in the catalog.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    /// <summary>Update data from a product entity in the catalog.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        int id,
        [FromBody] ProductUpdateRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _productService.UpdateAsync(id, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>Performs a logical soft deletion on a product by its ID.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
