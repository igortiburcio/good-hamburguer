namespace GoodHamburger.Domain.Src;

public record Order(string id, string ClientName, decimal TotalPrice, List<Product> Products);
