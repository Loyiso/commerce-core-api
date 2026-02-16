using System.Security.Claims;
using IdentityService.API.Models.Auth;
using IdentityService.API.Repositories;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityService.API.Models;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IUserRepository _users;
    private readonly ILogger<AuthController> _logger;
    private readonly ILoggingApiClient _loggingApi;

    public AuthController(
        IAuthService auth,
        IUserRepository users,
        ILogger<AuthController> logger,
        ILoggingApiClient loggingApi)
    {
        _auth = auth;
        _users = users;
        _logger = logger;
        _loggingApi = loggingApi;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation("POST /auth/register started | Email={Email} | Ip={Ip}", request?.Email, ip);
        FireAndForgetLog("Information", "Register started", traceId, new()
        {
            ["Email"] = request?.Email,
            ["Ip"] = ip,
            ["UserAgent"] = ua
        });

        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                FireAndForgetLog("Warning", "Register failed due to invalid request", traceId, new()
                {
                    ["Email"] = request?.Email
                });
                return BadRequest(new { message = "Invalid request data." });
            }

            var result = await _auth.RegisterAsync(request, ua, ip, ct);

            _logger.LogInformation("POST /auth/register succeeded | Email={Email}", request?.Email);
            FireAndForgetLog("Information", "Register succeeded", traceId, new()
            {
                ["Email"] = request?.Email
            });

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "POST /auth/register conflict | Email={Email}", request?.Email);
            FireAndForgetLog("Warning", "Register conflict", traceId, new()
            {
                ["Email"] = request?.Email
            }, ex);

            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /auth/register failed | Email={Email}", request?.Email);
            FireAndForgetLog("Error", "Register failed", traceId, new()
            {
                ["Email"] = request?.Email
            }, ex);

            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation("POST /auth/login started | Email={Email} | Ip={Ip}", request?.EmailOrUsername, ip);
        FireAndForgetLog("Information", "Login started", traceId, new()
        {
            ["Email"] = request?.EmailOrUsername,
            ["Ip"] = ip,
            ["UserAgent"] = ua
        });

        try
        {
            if (request is null || string.IsNullOrWhiteSpace(request.EmailOrUsername) || string.IsNullOrWhiteSpace(request.Password))
            {
                FireAndForgetLog("Warning", "Login failed due to invalid request", traceId, new()
                {
                    ["Email"] = request?.EmailOrUsername
                });
                return BadRequest(new { message = "Invalid request data." });
            }

            var result = await _auth.LoginAsync(request, ua, ip, ct);

            _logger.LogInformation("POST /auth/login succeeded | Email={Email}", request?.EmailOrUsername);
            FireAndForgetLog("Information", "Login succeeded", traceId, new()
            {
                ["Email"] = request?.EmailOrUsername
            });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        { 
            _logger.LogWarning(ex, "POST /auth/login unauthorized | Email={Email}", request?.EmailOrUsername);
            FireAndForgetLog("Warning", "Login unauthorized", traceId, new()
            {
                ["Email"] = request?.EmailOrUsername
            }, ex);

            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /auth/login failed | Email={Email}", request?.EmailOrUsername);
            FireAndForgetLog("Error", "Login failed", traceId, new()
            {
                ["Email"] = request?.EmailOrUsername
            }, ex);

            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation("POST /auth/refresh started | Ip={Ip}", ip);
        FireAndForgetLog("Information", "Refresh started", traceId, new()
        {
            ["Ip"] = ip,
            ["UserAgent"] = ua
        });

        try
        {
            var result = await _auth.RefreshAsync(request, ua, ip, ct);

            _logger.LogInformation("POST /auth/refresh succeeded");
            FireAndForgetLog("Information", "Refresh succeeded", traceId);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "POST /auth/refresh unauthorized | Ip={Ip}", ip);
            FireAndForgetLog("Warning", "Refresh unauthorized", traceId, new()
            {
                ["Ip"] = ip
            }, ex);

            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /auth/refresh failed");
            FireAndForgetLog("Error", "Refresh failed", traceId, ex: ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var traceId = HttpContext.TraceIdentifier;

        _logger.LogInformation("POST /auth/logout started");
        FireAndForgetLog("Information", "Logout started", traceId);

        try
        {
            await _auth.LogoutAsync(request, ct);

            _logger.LogInformation("POST /auth/logout succeeded");
            FireAndForgetLog("Information", "Logout succeeded", traceId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST /auth/logout failed");
            FireAndForgetLog("Error", "Logout failed", traceId, ex: ex);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken ct)
    {
        var traceId = HttpContext.TraceIdentifier;

        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var userId))
        {
            _logger.LogWarning("GET /auth/me invalid token subject");
            FireAndForgetLog("Warning", "Me invalid token subject", traceId);
            return Unauthorized(new { message = "Invalid token subject." });
        }

        _logger.LogInformation("GET /auth/me started | UserId={UserId}", userId);
        FireAndForgetLog("Information", "Me started", traceId, new()
        {
            ["UserId"] = userId
        });

        try
        {
            var user = await _users.GetByIdAsync(userId, ct);
            if (user is null)
            {
                _logger.LogWarning("GET /auth/me user not found | UserId={UserId}", userId);
                FireAndForgetLog("Warning", "Me user not found", traceId, new()
                {
                    ["UserId"] = userId
                });
                return Unauthorized(new { message = "User not found." });
            }

            _logger.LogInformation("GET /auth/me succeeded | UserId={UserId}", userId);
            FireAndForgetLog("Information", "Me succeeded", traceId, new()
            {
                ["UserId"] = userId
            });

            return Ok(new MeResponse(user.Id, user.Email, user.Username, user.Role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET /auth/me failed | UserId={UserId}", userId);
            FireAndForgetLog("Error", "Me failed", traceId, new()
            {
                ["UserId"] = userId
            }, ex);

            return StatusCode(500, "Internal server error");
        }
    }

    private void FireAndForgetLog(
        string level,
        string message,
        string traceId,
        Dictionary<string, object?>? props = null,
        Exception? ex = null)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await _loggingApi.LogAsync(new LoggingApiRequest
                {
                    Service = "IdentityService.API",
                    Level = level,
                    Message = message,
                    TraceId = traceId,
                    Exception = ex?.ToString(),
                    Properties = props ?? new Dictionary<string, object?>()
                });
            }
            catch
            {
                // logging must never break auth endpoints
            }
        });
    }
}
