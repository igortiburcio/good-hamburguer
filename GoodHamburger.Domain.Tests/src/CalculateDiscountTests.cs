using GoodHamburger.Domain.Src;
using Xunit;

namespace GoodHamburger.Domain.Tests.Src;

public class CalculateDiscountTests
{
    private readonly CalculateDiscount _sut = new();

    [Fact]
    public void Execute_WhenHasHamburgerFriesAndDrink_ShouldApplyTwentyPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Burger", 5.00m, ProductType.Hamburger),
            new("2", "Batata frita", 2.00m, ProductType.Fries),
            new("3", "Refrigerante", 2.50m, ProductType.Drink)
        };

        var result = _sut.Execute(products);

        Assert.Equal(7.60m, result);
    }

    [Fact]
    public void Execute_WhenHasHamburgerAndDrink_ShouldApplyFifteenPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Egg", 4.50m, ProductType.Hamburger),
            new("2", "Refrigerante", 2.50m, ProductType.Drink)
        };

        var result = _sut.Execute(products);

        Assert.Equal(5.95m, result);
    }

    [Fact]
    public void Execute_WhenHasHamburgerAndFries_ShouldApplyTenPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Bacon", 7.00m, ProductType.Hamburger),
            new("2", "Batata frita", 2.00m, ProductType.Fries)
        };

        var result = _sut.Execute(products);

        Assert.Equal(8.10m, result);
    }

    [Fact]
    public void Execute_WhenHasNoDiscountCombination_ShouldReturnSubtotal()
    {
        var products = new List<Product>
        {
            new("1", "Batata frita", 2.00m, ProductType.Fries),
            new("2", "Refrigerante", 2.50m, ProductType.Drink)
        };

        var result = _sut.Execute(products);

        Assert.Equal(4.50m, result);
    }

    [Fact]
    public void Execute_WhenListIsEmpty_ShouldReturnZero()
    {
        var result = _sut.Execute([]);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Execute_WhenProductsIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Execute(null!));
    }
}
