using LoggingService.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoggingService.API.Data;

public sealed class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Level).HasMaxLength(32);
            e.Property(x => x.Message).HasMaxLength(4000);
            e.Property(x => x.Service).HasMaxLength(128);
            e.Property(x => x.CorrelationId).HasMaxLength(128);
        });
    }
}
