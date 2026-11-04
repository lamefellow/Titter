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
    private IPostRepository _postRepository;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,  IPostRepository postRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _postRepository = postRepository;
    }

    [HttpGet("account/{username}")]
    public async Task<IActionResult> Profile(string username, int page = 1)
    {
        if (page < 1) page = 1;
        int pageSize = 20;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound();

        var userPosts = await _postRepository.GetPostsByUserAsync(user.Id, page, pageSize);

        var viewModel = new UserProfileViewModel
        {
            Username = user.UserName,
            CreatedAt = user.CreatedAt,
            Posts = userPosts
        };

        ViewData["Page"] = page;
        return View(viewModel);
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
            model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Posts");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
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