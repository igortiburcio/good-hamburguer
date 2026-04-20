using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Application.Src.UseCases;
using GoodHamburger.Domain.Src;
using NSubstitute;
using Xunit;

namespace GoodHamburger.Application.Tests.Src.UseCases;

public class MenuUseCaseTests
{
    [Fact]
    public async Task GetMenuAsync_WhenRepositoryReturnsProducts_ShouldReturnProducts()
    {
        var repository = Substitute.For<IProductRepository>();
        var expected = new List<Product>
        {
            new("1", "X Burger", 5.00m, "Hamburger"),
            new("2", "Batata frita", 2.00m, "Fries")
        };

        repository.GetAllProductsAsync().Returns(Task.FromResult(expected));
        var sut = new MenuUseCase(repository);

        var result = await sut.GetMenuAsync();

        Assert.Equal(expected.Count, result.Count);
        Assert.Equal(expected[0], result[0]);
        Assert.Equal(expected[1], result[1]);
    }
}
