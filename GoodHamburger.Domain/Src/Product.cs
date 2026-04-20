namespace GoodHamburger.Domain.Src;

public enum ProductType
{
    Hamburger,
    Fries,
    Drink
}

public record Product(string id, string Name, decimal Price, ProductType Type);
