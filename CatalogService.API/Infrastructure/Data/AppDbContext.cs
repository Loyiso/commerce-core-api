
using CatalogService.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.API.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
}
