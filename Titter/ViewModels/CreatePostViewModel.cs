using System.ComponentModel.DataAnnotations;

namespace Titter.ViewModels;

public class CreatePostViewModel
{
    [Required]
    [StringLength(1000, ErrorMessage = "Post can't be longer than 1000 characters.")]
    public string Content { get; set; }

    public bool LockReplies { get; set; }
}