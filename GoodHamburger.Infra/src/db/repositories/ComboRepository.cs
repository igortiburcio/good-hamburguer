using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Domain.Src;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infra.Src.Db.Repositories;

public class ComboRepository(AppDbContext dbContext) : IComboRepository
{
    public async Task<List<Combo>> GetActiveCombosAsync()
    {
        var combos = await dbContext.Combos
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Include(c => c.ComboCategories)
            .ThenInclude(cc => cc.Category)
            .ToListAsync();

        return combos.Select(c => new Combo(
            c.Id.ToString(),
            c.Name,
            c.DiscountPercent,
            c.ComboCategories.Select(cc => cc.Category.Name).ToList())).ToList();
    }
}
