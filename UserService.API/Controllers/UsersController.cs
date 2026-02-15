using Microsoft.AspNetCore.Mvc;
using UserService.API.Contracts;
using UserService.API.Services;

namespace UserService.API.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService; 
    private readonly ILoggingApiClient _loggingApi;

    public UsersController(
        IUserService userService, 
        ILoggingApiClient loggingApi)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService)); 
        _loggingApi = loggingApi ?? throw new ArgumentNullException(nameof(loggingApi));
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetAll(CancellationToken ct)
    { 
        FireAndForgetLog("Information", "GET /users started");

        try
        {
            var result = await _userService.GetAllAsync(ct);
 
            FireAndForgetLog("Information", "GET /users succeeded", new()
            {
                ["Count"] = result.Count
            });

            return Ok(result);
        }
        catch (Exception ex)
        {  
            FireAndForgetLog("Error", "GET /users failed", new()
            {
                ["Error"] = ex.Message
            }, ex);

            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "GET /users/{Id} started", new() { ["Id"] = id });

        try
        {
            var user = await _userService.GetByIdAsync(id, ct);

            if (user is null)
            { 
                FireAndForgetLog("Warning", "GET /users/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            }
 
            FireAndForgetLog("Information", "GET /users/{Id} succeeded", new() { ["Id"] = id });

            return Ok(user);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "GET /users/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest request, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "POST /users started", new() { ["Email"] = request?.Email });

        try
        {
            if (request is null)
            { 
                FireAndForgetLog("Warning", "POST /users bad request | Request body is null");
                return BadRequest("Request body cannot be null");
            }

            var created = await _userService.CreateAsync(request, ct);
 
            FireAndForgetLog("Information", "POST /users succeeded", new()
            {
                ["Id"] = created.Id,
                ["Email"] = created.Email
            });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "POST /users failed", new() { ["Email"] = request?.Email }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UserUpdateRequest request, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "PUT /users/{Id} started", new() { ["Id"] = id });

        try
        {
            var updated = await _userService.UpdateAsync(id, request, ct);

            if (updated is null)
            { 
                FireAndForgetLog("Warning", "PUT /users/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            }
 
            FireAndForgetLog("Information", "PUT /users/{Id} succeeded", new() { ["Id"] = id });

            return Ok(updated);
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "PUT /users/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    { 
        FireAndForgetLog("Information", "DELETE /users/{Id} started", new() { ["Id"] = id });

        try
        {
            var deleted = await _userService.DeleteAsync(id, ct);

            if (!deleted)
            { 
                FireAndForgetLog("Warning", "DELETE /users/{Id} not found", new() { ["Id"] = id });
                return NotFound();
            }
 
            FireAndForgetLog("Information", "DELETE /users/{Id} succeeded", new() { ["Id"] = id });

            return NoContent();
        }
        catch (Exception ex)
        { 
            FireAndForgetLog("Error", "DELETE /users/{Id} failed", new() { ["Id"] = id }, ex);
            return StatusCode(500, "Internal server error");
        }
    }

    private void FireAndForgetLog(
        string level,
        string message,
        Dictionary<string, object?>? props = null,
        Exception? ex = null)
    {
        // Capture what you need NOW; never touch HttpContext inside the Task later.
        var traceId = HttpContext.TraceIdentifier;

        _ = Task.Run(async () =>
        {
            try
            {
                await _loggingApi.LogAsync(new LoggingApiRequest
                {
                    Service = "UserService.API",
                    Level = level,
                    Message = message,
                    TraceId = traceId,
                    Exception = ex?.ToString(),
                    Properties = props ?? new Dictionary<string, object?>()
                });
            }
            catch
            {
                // Swallow: logging must never break requests.
            }
        });
    }
}
