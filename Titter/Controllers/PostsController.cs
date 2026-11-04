using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Titter.Infrastructure.Models;
using Titter.Infrastructure.Repositories;
using Titter.ViewModels;

namespace Titter.Controllers;

public class PostsController : Controller
{
    private readonly IPostRepository _postRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostsController(IPostRepository postRepository, UserManager<ApplicationUser> userManager)
    {
        _postRepository = postRepository;
        _userManager = userManager;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;
        
        var posts = await _postRepository.GetPostsPageAsync(page, 20);
        ViewData["Page"] = page;
        
        return View(posts);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Index));

        var post = new Post
        {
            Content = model.Content,
            UserId = _userManager.GetUserId(User)!,
            Locked = model.LockReplies
        };

        await _postRepository.AddPostAsync(post);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet("posts/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        var post = await _postRepository.GetPostAsync(id);
        if (post == null) return NotFound();
        
        post.Replies = post.Replies.OrderBy(r => r.CreatedAt).ToList();

        return View(post);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReply(string parentPostId, string content)
    {
        if (string.IsNullOrEmpty(content) || content.Length > 1000)
        {
            return RedirectToAction(nameof(Details), new { id = parentPostId });
        }
        
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();
        
        var parent = await _postRepository.GetPostAsync(parentPostId);
        if (parent == null) return NotFound();

        var root = parent.ParentPost ?? parent;
        if (root.Locked && root.UserId != userId)
            return Forbid();

        var reply = new Post
        {
            Content = content,
            UserId = userId,
            ParentPostId = parentPostId
        };

        await _postRepository.AddReplyAsync(reply);
        return RedirectToAction(nameof(Details), new { id = parentPostId });
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var post = await _postRepository.GetPostAsync(id);
        if (post == null || post.UserId != _userManager.GetUserId(User)) 
            return NotFound();

        await _postRepository.DeletePostAsync(id);
        
        if (!string.IsNullOrEmpty(post.ParentPostId))
        {
            return RedirectToAction(nameof(Details), new { id = post.ParentPostId });
        }
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleLike(string id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();
        
        var post = await _postRepository.GetPostAsync(id);
        if (post == null) return NotFound();

        await _postRepository.ToggleLikeAsync(id, userId);

        if (Request.Headers.ContainsKey("HX-Request"))
        {
            var updated = await _postRepository.GetPostAsync(id);
            return PartialView("_Post", updated);
        }

        return RedirectToAction(nameof(Index));   
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(string id)
    {
        var post = await _postRepository.GetPostAsync(id);
        if (post == null) return NotFound();
        if (post.ParentPostId != null) return BadRequest();
        if (post.UserId != _userManager.GetUserId(User)) return Forbid();

        await _postRepository.SetLockedAsync(id, !post.Locked);
        return RedirectToAction(nameof(Details), new { id });
    }
}
