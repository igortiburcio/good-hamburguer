using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Domain.Src;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class ComboRepository(AppDbContext dbContext, HybridCache hybridCache) : IComboRepository
{
    private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<List<Combo>> GetActiveCombosAsync()
    {
        return await hybridCache.GetOrCreateAsync(
            "combos:active:v1",
            async _ =>
            {
                var combos = await dbContext.Combos
                    .AsNoTracking()
                    .Where(c => c.IsActive)
                    .Include(c => c.ComboCategories)
                    .ThenInclude(cc => cc.Category)
                    .ToListAsync(cancellationToken: _);

                return combos.Select(c => new Combo(
                    c.Id.ToString(),
                    c.Name,
                    c.DiscountPercent,
                    c.ComboCategories.Select(cc => cc.Category.Name).ToList())).ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = s_cacheDuration,
                LocalCacheExpiration = s_cacheDuration
            });
    }
}
