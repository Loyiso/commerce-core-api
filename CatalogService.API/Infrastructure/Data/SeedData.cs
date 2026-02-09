
using CatalogService.API.Domain.Entities;
using CatalogService.API.Infrastructure.Data;

namespace CatalogService.API.Infrastructure.Data;

public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (context.Products.Any()) return;

        var electronics = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        context.Categories.Add(electronics);

        context.Products.AddRange(
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 15",
                Description = "Latest smartphone",
                Price = 19999,
                ImageUrl = "https://via.placeholder.com/300x300.png?text=iPhone+15",
                CategoryId = electronics.Id
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Gaming Laptop",
                Description = "High performance laptop",
                Price = 25999,
                ImageUrl = "https://via.placeholder.com/300x300.png?text=Gaming+Laptop",
                CategoryId = electronics.Id
            }
        );

        context.SaveChanges();
    }
}
