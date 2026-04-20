namespace GoodHamburger.Domain.Src;

public class CalculateDiscount
{
    public decimal Execute(List<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var subtotal = products.Sum(p => p.Price);
        var discount = GetDiscount(products, subtotal);

        return subtotal - discount;
    }

    private static decimal GetDiscount(List<Product> products, decimal subtotal)
    {
        var hasHamburger = products.Any(p => p.Type == ProductType.Hamburger);
        var hasFries = products.Any(p => p.Type == ProductType.Fries);
        var hasDrink = products.Any(p => p.Type == ProductType.Drink);

        if (hasHamburger && hasFries && hasDrink)
        {
            return subtotal * 0.20m;
        }

        if (hasHamburger && hasDrink)
        {
            return subtotal * 0.15m;
        }

        if (hasHamburger && hasFries)
        {
            return subtotal * 0.10m;
        }

        return 0m;
    }
}
