using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Titter.Infrastructure.Models;
using Titter.Infrastructure.Repositories;
using Titter.ViewModels;

namespace Titter.Controllers;

public class UserController : Controller
{
    private UserManager<ApplicationUser> _userManager;
    private IPostRepository _postRepository;

    public UserController(UserManager<ApplicationUser> userManager, IPostRepository postRepository)
    {
        _userManager =  userManager;
        _postRepository = postRepository;
    }
    
    [HttpGet("u/{username}")]
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
}