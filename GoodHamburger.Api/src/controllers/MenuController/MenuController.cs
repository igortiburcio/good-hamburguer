
using Microsoft.AspNetCore.Mvc;
using GoodHamburger.Application.Src.UseCases;

namespace GoodHamburger.Api.Src.Controllers.MenuController;

[ApiController]
[Route("api/menu")]
public class MenuController(ILogger<MenuController> logger, MenuUseCase _menuUseCase) : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> GetMenu()
    {
        logger.LogInformation("Fetching menu items");

        var menu = await _menuUseCase.GetMenuAsync();

        var response = menu.Select(item => new GetMenuDto
        {
            Id = item.id,
            Name = item.Name,
            Price = item.Price,
            Category = _menuUseCase.ParseProductType(item.Type)
        }).ToList();

        return Ok(response);
    }
}
