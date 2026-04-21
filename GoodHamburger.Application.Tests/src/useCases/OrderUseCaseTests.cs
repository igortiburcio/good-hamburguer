using GoodHamburger.Application.Src.Errors;
using GoodHamburger.Application.Src.Repositories;
using GoodHamburger.Application.Src.UseCases;
using GoodHamburger.Domain.Src;
using NSubstitute;
using Xunit;

namespace GoodHamburger.Application.Tests.Src.UseCases;

public class OrderUseCaseTests
{
    private static readonly List<Combo> DefaultCombos =
    [
        new("1", "Combo Good Hamburger", 20m, ["Hamburger", "Fries", "Drink"]),
        new("2", "Combo Good Drink", 15m, ["Hamburger", "Drink"]),
        new("3", "Combo Good Fries", 10m, ["Hamburger", "Fries"])
    ];

    [Fact]
    public async Task CreateAsync_WhenValidInput_ShouldCreateOrderAndReturnTotals()
    {
        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        var hamburger = new Product("h1", "X Burger", 5.00m, "Hamburger");
        var fries = new Product("f1", "Batata frita", 2.00m, "Fries");

        productRepository.GetAllProductsAsync()
            .Returns(Task.FromResult(new List<Product> { hamburger, fries }));

        orderRepository.CreateAsync(Arg.Any<Order>())
            .Returns(callInfo =>
            {
                var input = callInfo.Arg<Order>();
                return Task.FromResult(input with { id = "order-1" });
            });

        var result = await sut.CreateAsync("  Igor  ", ["h1", "f1"]);

        Assert.Equal("order-1", result.Id);
        Assert.Equal("Igor", result.ClientName);
        Assert.Equal(7.00m, result.Subtotal);
        Assert.Equal(0.70m, result.Discount);
        Assert.Equal(6.30m, result.TotalFinal);

        await orderRepository.Received(1)
            .CreateAsync(Arg.Is<Order>(o =>
                o.ClientName == "Igor" &&
                o.Subtotal == 7.00m &&
                o.Discount == 0.70m &&
                o.TotalPrice == 6.30m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenClientNameIsInvalid_ShouldThrowInvalidOrderException(string clientName)
    {
        var sut = CreateSutWithMenu(out _, out _,
            new Product("h1", "X Burger", 5.00m, "Hamburger"));

        await Assert.ThrowsAsync<InvalidOrderException>(() => sut.CreateAsync(clientName, ["h1"]));
    }

    [Fact]
    public async Task CreateAsync_WhenProductIdsIsEmpty_ShouldThrowInvalidOrderException()
    {
        var sut = CreateSutWithMenu(out _, out _,
            new Product("h1", "X Burger", 5.00m, "Hamburger"));

        await Assert.ThrowsAsync<InvalidOrderException>(() => sut.CreateAsync("Igor", []));
    }

    [Fact]
    public async Task CreateAsync_WhenProductIdsHasDuplicateIds_ShouldThrowDuplicateOrderItemsException()
    {
        var sut = CreateSutWithMenu(out _, out _,
            new Product("h1", "X Burger", 5.00m, "Hamburger"));

        await Assert.ThrowsAsync<DuplicateOrderItemsException>(() => sut.CreateAsync("Igor", ["h1", "H1"]));
    }

    [Fact]
    public async Task CreateAsync_WhenProductIsNotInMenu_ShouldThrowInvalidOrderException()
    {
        var sut = CreateSutWithMenu(out _, out _,
            new Product("h1", "X Burger", 5.00m, "Hamburger"));

        await Assert.ThrowsAsync<InvalidOrderException>(() => sut.CreateAsync("Igor", ["missing"]));
    }

    [Fact]
    public async Task CreateAsync_WhenHasDuplicateProductTypes_ShouldThrowDuplicateOrderItemsException()
    {
        var sut = CreateSutWithMenu(out _, out _,
            new Product("h1", "X Burger", 5.00m, "Hamburger"),
            new Product("h2", "X Bacon", 7.00m, "Hamburger"));

        await Assert.ThrowsAsync<DuplicateOrderItemsException>(() => sut.CreateAsync("Igor", ["h1", "h2"]));
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ShouldReturnMappedOrder()
    {
        var order = new Order(
            "o-1",
            "Igor",
            7.00m,
            0.70m,
            6.30m,
            [new Product("h1", "X Burger", 5.00m, "Hamburger")]);

        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.GetByIdAsync("o-1").Returns(Task.FromResult<Order?>(order));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        var result = await sut.GetByIdAsync("o-1");

        Assert.Equal("o-1", result.Id);
        Assert.Equal(7.00m, result.Subtotal);
        Assert.Equal(0.70m, result.Discount);
        Assert.Equal(6.30m, result.TotalFinal);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.GetByIdAsync("missing").Returns(Task.FromResult<Order?>(null));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => sut.GetByIdAsync("missing"));
    }

    [Fact]
    public async Task GetAllAsync_WhenOrdersExist_ShouldReturnMappedList()
    {
        var orders = new List<Order>
        {
            new(
                "o-1",
                "Igor",
                7.00m,
                0.70m,
                6.30m,
                [new Product("h1", "X Burger", 5.00m, "Hamburger")]),
            new(
                "o-2",
                "Ana",
                9.50m,
                1.90m,
                7.60m,
                [
                    new Product("h1", "X Burger", 5.00m, "Hamburger"),
                    new Product("f1", "Batata frita", 2.00m, "Fries"),
                    new Product("d1", "Refrigerante", 2.50m, "Drink")
                ])
        };

        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.GetAllAsync().Returns(Task.FromResult(orders));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        var result = await sut.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("o-1", result[0].Id);
        Assert.Equal(7.60m, result[1].TotalFinal);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderExistsAndInputIsValid_ShouldUpdateOrderAndTotals()
    {
        var existing = new Order(
            "o-1",
            "Old",
            5.00m,
            0m,
            5.00m,
            [new Product("h1", "X Burger", 5.00m, "Hamburger")]);

        var hamburger = new Product("h1", "X Burger", 5.00m, "Hamburger");
        var drink = new Product("d1", "Refrigerante", 2.50m, "Drink");

        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();

        orderRepository.GetByIdAsync("o-1").Returns(Task.FromResult<Order?>(existing));
        productRepository.GetAllProductsAsync().Returns(Task.FromResult(new List<Product> { hamburger, drink }));
        orderRepository.UpdateAsync(Arg.Any<Order>())
            .Returns(callInfo => Task.FromResult<Order?>(callInfo.Arg<Order>()));

        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        var result = await sut.UpdateAsync("o-1", "  New  ", ["h1", "d1"]);

        Assert.Equal("New", result.ClientName);
        Assert.Equal(7.50m, result.Subtotal);
        Assert.Equal(1.125m, result.Discount);
        Assert.Equal(6.375m, result.TotalFinal);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDoesNotExist_ShouldThrowResourceNotFoundException()
    {
        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.GetByIdAsync("missing").Returns(Task.FromResult<Order?>(null));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => sut.UpdateAsync("missing", "Igor", ["h1"]));
    }

    [Fact]
    public async Task DeleteAsync_WhenDeleteReturnsTrue_ShouldSucceed()
    {
        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.DeleteAsync("o-1").Returns(Task.FromResult(true));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        var exception = await Record.ExceptionAsync(() => sut.DeleteAsync("o-1"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteAsync_WhenDeleteReturnsFalse_ShouldThrowResourceNotFoundException()
    {
        var orderRepository = Substitute.For<IOrderRepository>();
        var productRepository = Substitute.For<IProductRepository>();
        orderRepository.DeleteAsync("missing").Returns(Task.FromResult(false));
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        var discountCalculator = new DiscountCalculator();
        var sut = new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => sut.DeleteAsync("missing"));
    }

    private static OrderUseCase CreateSutWithMenu(
        out IOrderRepository orderRepository,
        out IProductRepository productRepository,
        params Product[] menu)
    {
        orderRepository = Substitute.For<IOrderRepository>();
        productRepository = Substitute.For<IProductRepository>();
        var comboRepository = Substitute.For<IComboRepository>();
        comboRepository.GetActiveCombosAsync().Returns(Task.FromResult(DefaultCombos));
        productRepository.GetAllProductsAsync().Returns(Task.FromResult(menu.ToList()));
        var discountCalculator = new DiscountCalculator();
        return new OrderUseCase(orderRepository, productRepository, comboRepository, discountCalculator);
    }
}
