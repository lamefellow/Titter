namespace Titter.ViewModels;

public class UserProfileViewModel
{
    public string Username { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Titter.Infrastructure.Models.Post> Posts { get; set; } = new();
}