using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; 
using UserService.API.Data;
using UserService.API.Repositories;
using UserService.API.Services;
 
var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddHttpClient<ILoggingApiClient, LoggingApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["LoggingApi:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(2);  
});
 
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
   
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService.API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter: Bearer {your JWT token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseInMemoryDatabase("UsersDb"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();

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
  
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    DbSeeder.Seed(db);
}
 
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
