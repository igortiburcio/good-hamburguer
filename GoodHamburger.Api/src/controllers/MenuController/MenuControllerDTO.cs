namespace GoodHamburger.Api.Src.Controllers.MenuController;

public record GetMenuDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required string Category { get; set; }
}
