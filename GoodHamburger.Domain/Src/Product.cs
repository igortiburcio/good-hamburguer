namespace GoodHamburger.Domain.Src;

public record Product
{
    public string Id { get; init; }
    public string Name { get; init; }
    private Money PriceInMoney { get; init; }
    public decimal Price => PriceInMoney.Amount;
    public string Category { get; init; }

    public Product(string id, string name, decimal price, string category)
    {
        Id = id;
        Name = name;
        PriceInMoney = new Money(price);
        Category = category;
    }
}
