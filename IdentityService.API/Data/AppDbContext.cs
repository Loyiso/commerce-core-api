using IdentityService.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.API.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.Username).IsUnique();

            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.Username).HasMaxLength(64);
            e.Property(x => x.PasswordHash).HasMaxLength(512);
            e.Property(x => x.Role).HasMaxLength(64);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.TokenHash).IsUnique();

            e.Property(x => x.TokenHash).HasMaxLength(128);
            e.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);

            e.HasOne(x => x.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
