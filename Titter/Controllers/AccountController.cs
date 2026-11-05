using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Titter.Infrastructure.Models;
using Titter.Infrastructure.Repositories;
using Titter.ViewModels;

namespace Titter.Controllers;

public class AccountController : Controller
{
    private UserManager<ApplicationUser> _userManager;
    private SignInManager<ApplicationUser> _signInManager;

    private string? ValidateUsername(string username)
    {
        if (username.Length < 3 || username.Length > 20)
            return "Username must be between 3 and 20 characters long.";
        return null;
    }
    
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    [Route("account/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [Route("account/login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var result = await _signInManager.PasswordSignInAsync(
            model.Username?.Trim() ?? string.Empty, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Posts");

        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View(model);
    }
    
    [HttpGet]
    [Route("account/register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("account/register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var name = model.Username?.Trim() ?? string.Empty;

        var usernameError = ValidateUsername(name);
        if (usernameError != null)
            ModelState.AddModelError(string.Empty, usernameError);

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser { UserName = name };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Posts");
        }

        foreach (var identityError in result.Errors)
        {
            ModelState.AddModelError(string.Empty, identityError.Description);
        }

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Posts");
    }
}