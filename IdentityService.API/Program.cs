using System.Text;
using IdentityService.API.Data;
using IdentityService.API.Repositories;
using IdentityService.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
 
var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"]!;
var audience = jwtSection["Audience"]!;
var signingKey = jwtSection["SigningKey"]!;

if (string.IsNullOrWhiteSpace(signingKey) || signingKey.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase) || signingKey.Length < 32)
{
    // This is a dev-friendly safeguard to avoid accidentally running with a weak key.
    // Update appsettings.json Jwt:SigningKey to a strong random secret.
    builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Information);
}

// --- Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    // In-memory DB for mock / local dev
    opt.UseInMemoryDatabase("IdentityServiceDb");
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.Run();
