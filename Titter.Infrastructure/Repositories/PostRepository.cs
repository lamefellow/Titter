using Microsoft.EntityFrameworkCore;
using Titter.Infrastructure.Models;

namespace Titter.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync()
    {
        return await _context.Posts
            .Where(p => p.ParentPostId == null)
            .Include(p => p.User)
            .Include(p => p.Replies)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPostsByUserAsync(string id, int page, int pageSize)
    {
        return await _context.Posts
            .Where(p => p.ParentPostId == null)
            .Where(p => p.UserId == id)
            .Include(p => p.User)
            .Include(p => p.Replies)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetPostsPageAsync(int page, int pageSize)
    {
        return await _context.Posts
            .Where(p => p.ParentPostId == null)
            .Include(p => p.User)
            .Include(p => p.Replies)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.IsPinned)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Post?> GetPostAsync(string id)
    {
        return await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.ParentPost)
            .Include(p => p.Replies).ThenInclude(r => r.User)
            .Include(p => p.Replies).ThenInclude(r => r.Likes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    
    public async Task<Post> AddPostAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task ToggleLikeAsync(string id, string userId)
    {
        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == id && pl.UserId == userId);

        if (existingLike != null)
        {
            _context.PostLikes.Remove(existingLike);
        }
        else
        {
            _context.PostLikes.Add(new PostLike 
            { 
                PostId = id, 
                UserId = userId 
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Post> UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task DeletePostAsync(string id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post != null)
        {
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Post> AddReplyAsync(Post reply)
    {
        _context.Posts.Add(reply);
        await _context.SaveChangesAsync();
        return reply;
    }

    public async Task<bool> SetLockedAsync(string id, bool locked)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return false;
        
        post.Locked = locked;
        await _context.SaveChangesAsync();

        return true;
    }
}