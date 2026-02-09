
using CatalogService.API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
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
        var result = await _service.GetAsync(search, sortBy, desc, page, pageSize);

        Response.Headers["Cache-Control"] = "public,max-age=60";

        return Ok(result);
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) return NotFound();

        Response.Headers["Cache-Control"] = "public,max-age=60";

        return Ok(product);
    }
}
