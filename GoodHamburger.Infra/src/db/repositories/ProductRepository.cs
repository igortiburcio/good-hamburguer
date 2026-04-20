using Microsoft.EntityFrameworkCore;
using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
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
