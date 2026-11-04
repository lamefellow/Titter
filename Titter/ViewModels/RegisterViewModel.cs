using System.ComponentModel.DataAnnotations;

namespace Titter.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the rules.")]
    public bool AcceptRules { get; set; }
}