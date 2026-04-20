using GoodHamburger.Domain.Src;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Application.Src.UseCases;

public class MenuUseCase(IProductRepository productRepository)
{
    public async Task<List<Product>> GetMenuAsync()
    {
        return await productRepository.GetAllProductsAsync();
    }

    public string ParseProductType(ProductType category)
    {
        return category switch
        {
            ProductType.Hamburger => "Hamburger",
            ProductType.Fries => "Fries",
            ProductType.Drink => "Drink",
            _ => throw new InvalidOperationException($"Unknown product category '{category}'.")
        };
    }
}
