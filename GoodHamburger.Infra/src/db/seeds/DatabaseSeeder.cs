using GoodHamburger.Infra.Src.Db.Entities;

namespace GoodHamburger.Infra.Src.Db;

public static class DatabaseSeeder
{
    public static void SeedProducts(AppDbContext dbContext)
    {
        var hasAnyProduct = dbContext.Products.Any();

        if (hasAnyProduct)
        {
            return;
        }

        var products = new List<ProductEntity>
        {
            new() { Name = "X Burger", Price = 5.00m, Category = "Hamburger" },
            new() { Name = "X Egg", Price = 4.50m, Category = "Hamburger" },
            new() { Name = "X Bacon", Price = 7.00m, Category = "Hamburger" },
            new() { Name = "Batata frita", Price = 2.00m, Category = "Fries" },
            new() { Name = "Refrigerante", Price = 2.50m, Category = "Drink" }
        };

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }
}
