using GoodHamburger.Domain.Src;
using Xunit;

namespace GoodHamburger.Domain.Tests.Src;

public class CalculateDiscountTests
{
    private readonly CalculateDiscount _sut = new();
    private readonly List<Combo> _defaultCombos =
    [
        new("1", "Combo Good Hamburger", 20m, ["Hamburger", "Fries", "Drink"]),
        new("2", "Combo Good Drink", 15m, ["Hamburger", "Drink"]),
        new("3", "Combo Good Fries", 10m, ["Hamburger", "Fries"])
    ];

    [Fact]
    public void Execute_WhenHasHamburgerFriesAndDrink_ShouldApplyTwentyPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Burger", 5.00m, "Hamburger"),
            new("2", "Batata frita", 2.00m, "Fries"),
            new("3", "Refrigerante", 2.50m, "Drink")
        };

        var result = _sut.Execute(products, _defaultCombos);

        Assert.Equal(7.60m, result);
    }

    [Fact]
    public void Execute_WhenHasHamburgerAndDrink_ShouldApplyFifteenPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Egg", 4.50m, "Hamburger"),
            new("2", "Refrigerante", 2.50m, "Drink")
        };

        var result = _sut.Execute(products, _defaultCombos);

        Assert.Equal(5.95m, result);
    }

    [Fact]
    public void Execute_WhenHasHamburgerAndFries_ShouldApplyTenPercentDiscount()
    {
        var products = new List<Product>
        {
            new("1", "X Bacon", 7.00m, "Hamburger"),
            new("2", "Batata frita", 2.00m, "Fries")
        };

        var result = _sut.Execute(products, _defaultCombos);

        Assert.Equal(8.10m, result);
    }

    [Fact]
    public void Execute_WhenHasNoDiscountCombination_ShouldReturnSubtotal()
    {
        var products = new List<Product>
        {
            new("1", "Batata frita", 2.00m, "Fries"),
            new("2", "Refrigerante", 2.50m, "Drink")
        };

        var result = _sut.Execute(products, _defaultCombos);

        Assert.Equal(4.50m, result);
    }

    [Fact]
    public void Execute_WhenListIsEmpty_ShouldReturnZero()
    {
        var result = _sut.Execute([], _defaultCombos);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Execute_WhenProductsIsNull_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Execute(null!, _defaultCombos));
    }
}
