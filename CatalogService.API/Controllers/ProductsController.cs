using CatalogService.API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService service,
        ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Get(
        string? search,
        string? sortBy = "name",
        bool desc = false,
        int page = 1,
        int pageSize = 10)
    {
        _logger.LogInformation(
            "Fetching products. Search={Search}, SortBy={SortBy}, Desc={Desc}, Page={Page}, PageSize={PageSize}",
            search, sortBy, desc, page, pageSize);

        try
        {
            var result = await _service.GetAsync(search, sortBy, desc, page, pageSize);

            Response.Headers["Cache-Control"] = "public,max-age=60";

            _logger.LogInformation(
                "Products fetched successfully. TotalCount={TotalCount}",
                result?.TotalCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetById(Guid id)
    {
        _logger.LogInformation("Fetching product by Id={ProductId}", id);

        try
        {
            var product = await _service.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product not found. Id={ProductId}", id);
                return NotFound();
            }

            Response.Headers["Cache-Control"] = "public,max-age=60";

            _logger.LogInformation("Product retrieved successfully. Id={ProductId}", id);

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product Id={ProductId}", id);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
