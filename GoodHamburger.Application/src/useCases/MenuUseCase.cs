using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Domain.Src;

namespace GoodHamburger.Application.Src.UseCases;

public class MenuUseCase(IProductRepository productRepository)
{
    public async Task<List<Product>> GetMenuAsync()
    {
        return await productRepository.GetAllProductsAsync();
    }
}
