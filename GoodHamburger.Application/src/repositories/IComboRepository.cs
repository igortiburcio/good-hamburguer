using GoodHamburger.Domain.Src;

namespace GoodHamburger.Application.Src.Repositories;

public interface IComboRepository
{
    Task<List<Combo>> GetActiveCombosAsync();
}
