using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserService.API.Controllers;

public abstract class ApiBaseController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;

    protected ApiBaseController(IHttpContextAccessor httpContextAccessor, ILogger logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public ObjectResult Error()
        => StatusCode(500, "The server encountered an internal error and was unable to complete your request.");

    protected string RequestedUrl =>
        _httpContextAccessor.HttpContext?.Request?.Path.Value ?? "Unknown URL";

    protected string UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value
        ?? "Anonymous";

    protected string LogErrorMessage =>
        $"Error 500. The requested URL ({RequestedUrl}) encountered an issue.";

    [ApiExplorerSettings(IgnoreApi = true)]
    protected void LogRequest(string method, long? id = null)
    {
        _logger.LogInformation(
            "{Method} request started | Id: {Id} | Url: {Url} | User: {User}",
            method, id, RequestedUrl, UserName);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    protected void LogSuccess(string method, long? id = null)
    {
        _logger.LogInformation(
            "{Method} request succeeded | Id: {Id} | Url: {Url}",
            method, id, RequestedUrl);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    protected void LogError(Exception ex)
    {
        _logger.LogError(ex, "{Message} | Url: {Url} | User: {User}", LogErrorMessage, RequestedUrl, UserName);
    }
}
