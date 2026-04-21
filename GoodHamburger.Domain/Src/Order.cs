namespace GoodHamburger.Domain.Src;

public record Order
{
    public string Id { get; init; }
    public string ClientName { get; init; }
    private Money SubtotalInMoney { get; init; }
    public decimal Subtotal => SubtotalInMoney.Amount;
    private Money DiscountInMoney { get; init; }
    public decimal Discount => DiscountInMoney.Amount;
    private Money TotalPriceInMoney { get; init; }
    public decimal TotalPrice => TotalPriceInMoney.Amount;
    public List<Product> Products { get; init; }

    public Order(string id, string clientName, decimal subtotal, decimal discount, decimal totalPrice, List<Product> products)
    {
        Id = id;
        ClientName = clientName;
        SubtotalInMoney = new Money(subtotal);
        DiscountInMoney = new Money(discount);
        TotalPriceInMoney = new Money(totalPrice);
        Products = products;
    }
};
