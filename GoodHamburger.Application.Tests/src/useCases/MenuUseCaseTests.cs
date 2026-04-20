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
            new("1", "X Burger", 5.00m, ProductType.Hamburger),
            new("2", "Batata frita", 2.00m, ProductType.Fries)
        };

        repository.GetAllProductsAsync().Returns(Task.FromResult(expected));
        var sut = new MenuUseCase(repository);

        var result = await sut.GetMenuAsync();

        Assert.Equal(expected.Count, result.Count);
        Assert.Equal(expected[0], result[0]);
        Assert.Equal(expected[1], result[1]);
    }

    [Theory]
    [InlineData(ProductType.Hamburger, "Hamburger")]
    [InlineData(ProductType.Fries, "Fries")]
    [InlineData(ProductType.Drink, "Drink")]
    public void ParseProductType_WhenValidType_ShouldReturnExpectedLabel(ProductType type, string expected)
    {
        var repository = Substitute.For<IProductRepository>();
        var sut = new MenuUseCase(repository);

        var result = sut.ParseProductType(type);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ParseProductType_WhenInvalidType_ShouldThrowInvalidOperationException()
    {
        var repository = Substitute.For<IProductRepository>();
        var sut = new MenuUseCase(repository);

        Assert.Throws<InvalidOperationException>(() => sut.ParseProductType((ProductType)99));
    }
}
