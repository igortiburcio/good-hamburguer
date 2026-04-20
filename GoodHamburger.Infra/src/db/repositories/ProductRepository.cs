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
            .Include(entity => entity.Category)
            .Select(entity => new Product(
                entity.Id.ToString(),
                entity.Name,
                entity.Price,
                entity.Category.Name))
            .ToListAsync();

        return products;
    }
}
