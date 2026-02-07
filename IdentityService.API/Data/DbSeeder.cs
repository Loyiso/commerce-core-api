using IdentityService.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.API.Data;

public static class DbSeeder
{
    public static Task SeedAsync(AppDbContext db)
    {
        if (db.Users.Any())
            return Task.CompletedTask;

        var hasher = new PasswordHasher<AppUser>();

        var admin = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@demo.local",
            Username = "admin",
            Role = "Admin",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin@12345");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = "user@demo.local",
            Username = "user",
            Role = "User",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, "User@12345");

        db.Users.AddRange(admin, user);
        db.SaveChanges();
        return Task.CompletedTask;
    }
}
