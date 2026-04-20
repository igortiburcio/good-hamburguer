using GoodHamburger.Infra.Src.Db.Entities;

namespace GoodHamburger.Infra.Src.Db.Seeds;

public static class DatabaseSeeder
{
    public static void SeedProducts(AppDbContext dbContext)
    {
        SeedCategories(dbContext);
        SeedCombos(dbContext);

        var hasAnyProduct = dbContext.Products.Any();

        if (hasAnyProduct)
        {
            return;
        }

        var categoriesByName = dbContext.ProductCategories
            .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var products = new List<ProductEntity>
        {
            new() { Name = "X Burger", Price = 5.00m, CategoryId = categoriesByName["Hamburger"].Id },
            new() { Name = "X Egg", Price = 4.50m, CategoryId = categoriesByName["Hamburger"].Id },
            new() { Name = "X Bacon", Price = 7.00m, CategoryId = categoriesByName["Hamburger"].Id },
            new() { Name = "Batata frita", Price = 2.00m, CategoryId = categoriesByName["Fries"].Id },
            new() { Name = "Refrigerante", Price = 2.50m, CategoryId = categoriesByName["Drink"].Id }
        };

        dbContext.Products.AddRange(products);
        dbContext.SaveChanges();
    }

    private static void SeedCategories(AppDbContext dbContext)
    {
        if (dbContext.ProductCategories.Any())
        {
            return;
        }

        var categories = new List<ProductCategoryEntity>
        {
            new() { Name = "Hamburger" },
            new() { Name = "Fries" },
            new() { Name = "Drink" }
        };

        dbContext.ProductCategories.AddRange(categories);
        dbContext.SaveChanges();
    }

    private static void SeedCombos(AppDbContext dbContext)
    {
        if (dbContext.Combos.Any())
        {
            return;
        }

        var categoriesByName = dbContext.ProductCategories
            .ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var combos = new List<ComboEntity>
        {
            new() { Name = "Combo Good Hamburger", DiscountPercent = 20m, IsActive = true },
            new() { Name = "Combo Good Drink", DiscountPercent = 15m, IsActive = true },
            new() { Name = "Combo Good Fries", DiscountPercent = 10m, IsActive = true }
        };

        dbContext.Combos.AddRange(combos);
        dbContext.SaveChanges();

        var comboCategories = new List<ComboCategoryEntity>
        {
            new() { ComboId = combos[0].Id, CategoryId = categoriesByName["Hamburger"].Id },
            new() { ComboId = combos[0].Id, CategoryId = categoriesByName["Fries"].Id },
            new() { ComboId = combos[0].Id, CategoryId = categoriesByName["Drink"].Id },
            new() { ComboId = combos[1].Id, CategoryId = categoriesByName["Hamburger"].Id },
            new() { ComboId = combos[1].Id, CategoryId = categoriesByName["Drink"].Id },
            new() { ComboId = combos[2].Id, CategoryId = categoriesByName["Hamburger"].Id },
            new() { ComboId = combos[2].Id, CategoryId = categoriesByName["Fries"].Id }
        };

        dbContext.ComboCategories.AddRange(comboCategories);
        dbContext.SaveChanges();
    }
}
