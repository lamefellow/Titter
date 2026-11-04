using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Titter.Infrastructure.Models;

public class Post
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? ParentPostId { get; set; }
    public Post? ParentPost { get; set; }

    public bool Locked { get; set; } = false;
    
    public ICollection<Post> Replies { get; set; } = new List<Post>();
    
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}