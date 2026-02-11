using Microsoft.AspNetCore.Mvc;
using UserService.API.Contracts;
using UserService.API.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService service, ILogger<UsersController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("GET /users started");

        try
        {
            var result = await _service.GetAllAsync(ct);
            _logger.LogInformation("GET /users succeeded | Count: {Count}", result.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET /users failed");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("GET /users/{Id} started", id);

        try
        {
            var user = await _service.GetByIdAsync(id, ct);

            if (user is null)
            {
                _logger.LogWarning("GET /users/{Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("GET /users/{Id} succeeded", id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET /users/{Id} failed", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest request, CancellationToken ct)
    {
        _logger.LogInformation("POST /users started | Email: {Email}", request?.Email);

        try
        {
            var created = await _service.CreateAsync(request, ct);

            _logger.LogInformation("POST /users succeeded | Id: {Id} | Email: {Email}", created.Id, created.Email);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /users failed | Email: {Email}", request?.Email);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UserUpdateRequest request, CancellationToken ct)
    {
        _logger.LogInformation("PUT /users/{Id} started", id);

        try
        {
            var updated = await _service.UpdateAsync(id, request, ct);

            if (updated is null)
            {
                _logger.LogWarning("PUT /users/{Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("PUT /users/{Id} succeeded", id);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT /users/{Id} failed", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("DELETE /users/{Id} started", id);

        try
        {
            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
            {
                _logger.LogWarning("DELETE /users/{Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("DELETE /users/{Id} succeeded", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE /users/{Id} failed", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
