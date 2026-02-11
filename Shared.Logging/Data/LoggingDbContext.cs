using Microsoft.EntityFrameworkCore;
using Shared.Logging.Entities;

namespace Shared.Logging.Data;

public sealed class LoggingDbContext : DbContext
{
    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

    public DbSet<LogEntryEntity> Logs => Set<LogEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntryEntity>().HasIndex(x => x.Timestamp);
        modelBuilder.Entity<LogEntryEntity>().HasIndex(x => x.Service);
        base.OnModelCreating(modelBuilder);
    }
}
