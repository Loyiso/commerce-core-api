using LoggingService.API.Data;
using LoggingService.API.Repositories;
using LoggingService.API.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF InMemory
builder.Services.AddDbContext<LoggingDbContext>(opt =>
{
    opt.UseInMemoryDatabase("LoggingDb");
});

// App services
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "logging-service" }));

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
