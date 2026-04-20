using GoodHamburger.Domain.Src;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Repository;

public class ProductRepository(AppDbContext dbContext)
{
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .Select(entity => new Product(
                entity.Id.ToString(),
                entity.Name,
                entity.Price,
                ParseProductType(entity.Category)))
            .ToListAsync();

        return products;
    }

    private static ProductType ParseProductType(string category)
    {
        return category switch
        {
            "Hamburger" => ProductType.Hamburger,
            "Fries" => ProductType.Fries,
            "Drink" => ProductType.Drink,
            _ => throw new InvalidOperationException($"Unknown product category '{category}'.")
        };
    }
}
