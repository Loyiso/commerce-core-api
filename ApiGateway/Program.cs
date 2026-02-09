using Ocelot.DependencyInjection; 
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);
 
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
 
builder.Services.AddOcelot(builder.Configuration).AddPolly();

var app = builder.Build();
 
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "api-gateway" }));
 
await app.UseOcelot();

app.Run();
