using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Ocelot + Polly
builder.Services.AddOcelot(builder.Configuration).AddPolly();

var app = builder.Build();

// Serve a nice landing page from wwwroot (index.html)
app.UseDefaultFiles();   // looks for wwwroot/index.html
app.UseStaticFiles();    // serves static assets

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "api-gateway" }));

// IMPORTANT: Ocelot should be last
await app.UseOcelot();

app.Run();
