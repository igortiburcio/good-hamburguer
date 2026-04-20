using Microsoft.EntityFrameworkCore;
using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class ProductRepository(AppDbContext dbContext, HybridCache hybridCache) : IProductRepository
{
    private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await hybridCache.GetOrCreateAsync(
            "menu:all:v1",
            async _ => await dbContext.Products
                .AsNoTracking()
                .Include(entity => entity.Category)
                .Select(entity => new Product(
                    entity.Id.ToString(),
                    entity.Name,
                    entity.Price,
                    entity.Category.Name))
                .ToListAsync(cancellationToken: _),
            options: new HybridCacheEntryOptions
            {
                Expiration = s_cacheDuration,
                LocalCacheExpiration = s_cacheDuration
            });
    }
}
