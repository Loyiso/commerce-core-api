using System.Text;
using CatalogService.API.Application.Interfaces;
using CatalogService.API.Application.Services;
using CatalogService.API.Infrastructure.Data;
using CatalogService.API.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
 
using Shared.Logging.Dispatch;
using Shared.Logging.Extensions;
using Shared.Logging.Serilog;

var builder = WebApplication.CreateBuilder(args);
 
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Warning);
 
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? "IdentityService";
var audience = jwtSection["Audience"] ?? "IdentityServiceClients";
var signingKey = jwtSection["SigningKey"] ?? string.Empty;

var isWeakKey =
    string.IsNullOrWhiteSpace(signingKey) ||
    signingKey.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase) ||
    signingKey.Length < 32;

if (isWeakKey && builder.Environment.IsProduction())
{
    throw new InvalidOperationException("Jwt:SigningKey is missing/weak. Set a strong secret (>= 32 chars).");
}
 
builder.Services.AddHttpContextAccessor();

builder.Services.AddSharedInMemoryLogging(databaseName: "CatalogServiceLogsDb");

builder.Host.UseSerilog((ctx, services, lc) =>
{
    var dispatcher = services.GetRequiredService<ILogDispatcher>();

    lc.ReadFrom.Configuration(ctx.Configuration)
      .Enrich.FromLogContext()
      .WriteTo.Console()
      .WriteTo.Sink(new FireAndForgetInMemorySink(
          dispatcher,
          service: "CatalogService.API",
          environment: ctx.HostingEnvironment.EnvironmentName));
});
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("CatalogDb"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();
 
app.UseSerilogRequestLogging();
 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Seed(db);
}
 
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
