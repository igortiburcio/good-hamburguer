namespace GoodHamburger.Domain.Src;

public class CalculateDiscount
{
    public decimal Execute(List<Product> products, List<Combo> combos)
    {
        ArgumentNullException.ThrowIfNull(products);
        ArgumentNullException.ThrowIfNull(combos);

        var subtotal = products.Sum(p => p.Price);
        var discount = GetDiscount(products, subtotal, combos);

        return subtotal - discount;
    }

    private decimal GetDiscount(List<Product> products, decimal subtotal, List<Combo> combos)
    {
        var categoriesInOrder = new HashSet<string>(
            products.Select(p => p.Category),
            StringComparer.OrdinalIgnoreCase);

        var bestCombo = combos
            .Where(combo => combo.Categories.Count > 0)
            .Where(combo => combo.Categories.All(category => categoriesInOrder.Contains(category)))
            .OrderByDescending(combo => combo.DiscountPercent)
            .ThenByDescending(combo => combo.Categories.Count)
            .ThenBy(combo => combo.Id, StringComparer.Ordinal)
            .FirstOrDefault();

        if (bestCombo is null)
        {
            return 0m;
        }

        return subtotal * (bestCombo.DiscountPercent / 100m);
    }
}
