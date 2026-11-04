using Microsoft.AspNetCore.Mvc;

namespace Titter.Controllers;

public class PagesController : Controller
{
    [HttpGet("rules")]
    public IActionResult Rules() => View();

    [HttpGet("privacy")]
    public IActionResult Privacy() => View();
}