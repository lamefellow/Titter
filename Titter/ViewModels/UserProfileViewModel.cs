using Titter.Infrastructure.Models;

namespace Titter.ViewModels;

public class UserProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<Titter.Infrastructure.Models.Post> Posts { get; set; } = new List<Post>();
}