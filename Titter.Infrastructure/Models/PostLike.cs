namespace Titter.Infrastructure.Models;

public class PostLike
{
    public string PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}