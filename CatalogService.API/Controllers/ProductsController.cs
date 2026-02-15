using CatalogService.API.Application.Interfaces; 
using Microsoft.AspNetCore.Mvc;
using CatalogService.API.Application.Common;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service; 
    private readonly ILoggingApiClient _loggingApi;

    public ProductsController(
        IProductService service, 
        ILoggingApiClient loggingApi)
    {
        _service = service; 
        _loggingApi = loggingApi;
    }

    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> Get(
        string? search,
        string? sortBy = "name",
        bool desc = false,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    { 

        FireAndForgetLog("Information", "Fetching products", new()
        {
            ["Search"] = search,
            ["SortBy"] = sortBy,
            ["Desc"] = desc,
            ["Page"] = page,
            ["PageSize"] = pageSize
        });

        try
        {
            var result = await _service.GetAsync(search, sortBy, desc, page, pageSize, ct);

            Response.Headers["Cache-Control"] = "public,max-age=60";

            // If you’re returning paged result object, you can also expose paging headers:
            if (result is not null)
            {
                Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
                Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
                Response.Headers["X-Page-Size"] = result.PageSize.ToString();
                Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();
            }
 
            FireAndForgetLog("Information", "Products fetched successfully", new()
            {
                ["TotalCount"] = result?.TotalCount,
                ["Page"] = page,
                ["PageSize"] = pageSize
            });

            return Ok(result);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "Error occurred while fetching products", new()
            {
                ["Search"] = search,
                ["SortBy"] = sortBy,
                ["Desc"] = desc,
                ["Page"] = page,
                ["PageSize"] = pageSize
            }, ex);

            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    { 
        FireAndForgetLog("Information", "Fetching product by id", new()
        {
            ["ProductId"] = id
        });

        try
        {
            var product = await _service.GetByIdAsync(id, ct);

            if (product == null)
            {  
                FireAndForgetLog("Warning", "Product not found", new()
                {
                    ["ProductId"] = id
                });

                return NotFound();
            }

            Response.Headers["Cache-Control"] = "public,max-age=60"; 

            FireAndForgetLog("Information", "Product retrieved successfully", new()
            {
                ["ProductId"] = id
            });

            return Ok(product);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "Error occurred while fetching product", new()
            {
                ["ProductId"] = id
            }, ex);

            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    private void FireAndForgetLog(
        string level,
        string message,
        Dictionary<string, object?>? props = null,
        Exception? ex = null)
    {
        var traceId = HttpContext.TraceIdentifier;

        _ = Task.Run(async () =>
        {
            try
            {
                await _loggingApi.LogAsync(new LoggingApiRequest
                {
                    Service = "CatalogService.API",
                    Level = level,
                    Message = message,
                    TraceId = traceId,
                    Exception = ex?.ToString(),
                    Properties = props ?? new Dictionary<string, object?>()
                });
            }
            catch
            {
                // never break requests because central logging is down
            }
        });
    }
}
