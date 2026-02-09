using Microsoft.EntityFrameworkCore;
using UserService.API.Models;

namespace UserService.API.Data;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    public DbSet<UserProfile> Users => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>()
            .OwnsOne(u => u.Address);

        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.Email);
    }
}
