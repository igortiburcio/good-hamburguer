namespace GoodHamburger.Domain.Src;

public record Order(
    string id,
    string ClientName,
    decimal Subtotal,
    decimal Discount,
    decimal TotalPrice,
    List<Product> Products);
