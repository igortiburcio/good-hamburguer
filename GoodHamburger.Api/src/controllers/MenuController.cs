
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Src.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController(ILogger<MenuController> logger) : ControllerBase
{
    private static readonly string[] _menu = ["Hamburger", "Cheeseburger", "Veggie Burger", "Chicken Burger", "Fish Burger"];

    [HttpGet()]
    public IActionResult GetMenu()
    {
        logger.LogInformation("Fetching menu items");

        return Ok(_menu);
    }
}