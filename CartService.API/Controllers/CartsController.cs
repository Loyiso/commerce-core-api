using CartService.API.Contracts;
using CartService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.API.Controllers;

[ApiController]
[Route("carts")]
public class CartsController : ControllerBase
{
    private readonly ICartService _service;

    public CartsController(ICartService service) => _service = service;

    /// <summary>GET /carts</summary>
    [HttpGet]
    public async Task<ActionResult<List<CartResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _service.GetAllAsync(ct);
        return Ok(result);
    }

    /// <summary>GET /carts/{id}</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CartResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>GET /carts/user/{userId}</summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<CartResponse>>> GetByUserId(string userId, CancellationToken ct)
    {
        var result = await _service.GetByUserIdAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>POST /carts</summary>
    [HttpPost]
    public async Task<ActionResult<CartResponse>> Create([FromBody] CartCreateRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>PUT /carts/{id}</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CartResponse>> Update(Guid id, [FromBody] CartUpdateRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>DELETE /carts/{id}</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
