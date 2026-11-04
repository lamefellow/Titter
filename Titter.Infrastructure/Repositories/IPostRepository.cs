using Titter.Infrastructure.Models;

namespace Titter.Infrastructure.Repositories;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostsAsync();
    Task<IEnumerable<Post>> GetPostsPageAsync(int page, int pageSize);
    Task<Post?> GetPostAsync(string id);
    Task<Post> AddPostAsync(Post post);
    Task ToggleLikeAsync(string id, string userId);
    Task<Post> UpdatePostAsync(Post post);
    Task DeletePostAsync(string id);
    Task<Post> AddReplyAsync(Post reply);
    Task<bool> SetLockedAsync(string id, bool locked);
}