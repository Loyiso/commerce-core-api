using CartService.API.Contracts;
using CartService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.API.Controllers;

[ApiController]
[Route("carts")]
public class CartsController : ControllerBase
{
    private readonly ICartService _service; 
    private readonly ILoggingApiClient _loggingApi;

    public CartsController(ICartService service, ILoggingApiClient loggingApi)
    {
        _service = service; 
        _loggingApi = loggingApi;
    }

    /// <summary>GET /carts</summary>
    [HttpGet]
    public async Task<ActionResult<List<CartResponse>>> GetAll(CancellationToken ct)
    { 
        FireAndForgetLog("Information", "GET /carts started");

        try
        {
            var result = await _service.GetAllAsync(ct);
 
            FireAndForgetLog("Information", "GET /carts succeeded", new()
            {
                ["Count"] = result.Count
            });

            return Ok(result);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "GET /carts failed", ex: ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>GET /carts/{id}</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CartResponse>> GetById(Guid id, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "GET /carts/{Id} started", new() { ["Id"] = id });

        try
        {
            var result = await _service.GetByIdAsync(id, ct);

            if (result is null)
            { 
                FireAndForgetLog("Warning", "GET /carts/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            } 

            FireAndForgetLog("Information", "GET /carts/{Id} succeeded", new() { ["Id"] = id });

            return Ok(result);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "GET /carts/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>GET /carts/user/{userId}</summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<CartResponse>>> GetByUserId(string userId, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "GET /carts/user/{UserId} started", new() { ["UserId"] = userId });

        try
        {
            var result = await _service.GetByUserIdAsync(userId, ct);
 
            FireAndForgetLog("Information", "GET /carts/user/{UserId} succeeded", new()
            {
                ["UserId"] = userId,
                ["Count"] = result.Count
            });

            return Ok(result);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "GET /carts/user/{UserId} failed", new() { ["UserId"] = userId }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>POST /carts</summary>
    [HttpPost]
    public async Task<ActionResult<CartResponse>> Create([FromBody] CartCreateRequest request, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "POST /carts started", new() { ["UserId"] = request?.UserId });

        try
        {
            if (request is null)
            { 
                FireAndForgetLog("Warning", "POST /carts bad request | Request body is null");
                return BadRequest("Request body cannot be null");
            }

            var created = await _service.CreateAsync(request, ct);
 
            FireAndForgetLog("Information", "POST /carts succeeded", new()
            {
                ["Id"] = created.Id,
                ["UserId"] = created.UserId
            });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "POST /carts failed", new() { ["UserId"] = request?.UserId }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>PUT /carts/{id}</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CartResponse>> Update(Guid id, [FromBody] CartUpdateRequest request, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "PUT /carts/{Id} started", new() { ["Id"] = id });

        try
        {
            var updated = await _service.UpdateAsync(id, request, ct);

            if (updated is null)
            { 
                FireAndForgetLog("Warning", "PUT /carts/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            }
 
            FireAndForgetLog("Information", "PUT /carts/{Id} succeeded", new() { ["Id"] = id });

            return Ok(updated);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "PUT /carts/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>DELETE /carts/{id}</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "DELETE /carts/{Id} started", new() { ["Id"] = id });

        try
        {
            var ok = await _service.DeleteAsync(id, ct);

            if (!ok)
            { 
                FireAndForgetLog("Warning", "DELETE /carts/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            }
 
            FireAndForgetLog("Information", "DELETE /carts/{Id} succeeded", new() { ["Id"] = id });

            return NoContent();
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "DELETE /carts/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
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
                    Service = "CartService.API",
                    Level = level,
                    Message = message,
                    TraceId = traceId,
                    Exception = ex?.ToString(),
                    Properties = props ?? new Dictionary<string, object?>()
                });
            }
            catch
            {
                // never break the request pipeline because logging failed
            }
        });
    }
}
