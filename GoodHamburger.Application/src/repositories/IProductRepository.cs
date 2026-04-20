using GoodHamburger.Domain.Src;

namespace GoodHamburger.Application.Src.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllProductsAsync();

}
