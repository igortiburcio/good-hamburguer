namespace GoodHamburger.Domain.Src;

public record Combo(string Id, string Name, decimal DiscountPercent, List<string> Categories);
