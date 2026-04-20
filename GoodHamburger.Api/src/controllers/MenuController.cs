
using Microsoft.AspNetCore.Mvc;
using GoodHamburger.Application.Src.UseCases;

namespace GoodHamburger.Api.Src.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController(ILogger<MenuController> logger, MenuUseCase _menuUseCase) : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> GetMenu()
    {
        logger.LogInformation("Fetching menu items");

        var menu = await _menuUseCase.GetMenuAsync();

        return Ok(menu);
    }
}
