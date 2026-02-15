using LoggingService.API.DTOs;
using LoggingService.API.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LoggingService.API.Controllers;

[ApiController]
[Route("api/logs")]
public sealed class LogsController : ControllerBase
{
    private readonly ILogService _service;

    public LogsController(ILogService service) => _service = service;

    /// <summary>Add a log entry.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LogResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLogRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Also write to Serilog for observability of incoming logs
        Log.ForContext("SourceService", request.Service)
           .ForContext("CorrelationId", request.CorrelationId)
           .Information("Log received: {Level} - {Message}", request.Level, request.Message);

        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Get a single log by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var log = await _service.GetByIdAsync(id, ct);
        return log is null ? NotFound() : Ok(log);
    }

    /// <summary>Get logs with paging (ordered by newest first).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LogResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(pageNumber, pageSize, ct);

        // Helpful paging headers
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
        Response.Headers["X-Page-Size"] = result.PageSize.ToString();
        Response.Headers["X-Total-Pages"] = result.TotalPages.ToString();

        return Ok(result);
    }
}
