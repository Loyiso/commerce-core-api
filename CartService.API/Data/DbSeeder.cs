using CartService.API.Models;

namespace CartService.API.Data;

public static class DbSeeder
{
    public static void Seed(CartDbContext db)
    {
        if (db.Carts.Any()) return;

        var cart1 = new Cart { UserId = "user-1001" };
        cart1.Items.Add(new CartItem { CartId = cart1.Id, ProductId = "prod-001", Quantity = 2, UnitPrice = 199.99m });
        cart1.Items.Add(new CartItem { CartId = cart1.Id, ProductId = "prod-002", Quantity = 1, UnitPrice = 49.50m });

        var cart2 = new Cart { UserId = "user-1002" };
        cart2.Items.Add(new CartItem { CartId = cart2.Id, ProductId = "prod-003", Quantity = 3, UnitPrice = 15.00m });

        db.Carts.AddRange(cart1, cart2);
        db.SaveChanges();
    }
}
