using Microsoft.AspNetCore.Identity;

namespace Titter.Infrastructure.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}