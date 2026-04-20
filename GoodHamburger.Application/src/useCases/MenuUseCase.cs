using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Application.Src.UseCases;

public class MenuUseCase(IProductRepository productRepository)
{
    public async Task<List<Product>> GetMenuAsync()
    {
        return await productRepository.GetAllProductsAsync();
    }
}
