using UserService.API.Models;

namespace UserService.API.Data;

public static class DbSeeder
{
    public static void Seed(UsersDbContext db)
    {
        if (db.Users.Any()) return;

        var u1 = new UserProfile
        {
            FirstName = "Loyiso",
            LastName = "Nelani",
            Email = "loyiso@example.com",
            Phone = "+27 82 000 0000",
            Address = new Address
            {
                Line1 = "1 Main Road",
                City = "Johannesburg",
                Province = "Gauteng",
                PostalCode = "2000",
                Country = "South Africa"
            }
        };

        var u2 = new UserProfile
        {
            FirstName = "Thando",
            LastName = "Mokoena",
            Email = "thando@example.com",
            Phone = "+27 83 111 2222",
            Address = new Address
            {
                Line1 = "10 Park Ave",
                City = "Pretoria",
                Province = "Gauteng",
                PostalCode = "0002",
                Country = "South Africa"
            }
        };

        db.Users.AddRange(u1, u2);
        db.SaveChanges();
    }
}
