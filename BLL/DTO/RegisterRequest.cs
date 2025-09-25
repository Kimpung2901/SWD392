using System.ComponentModel.DataAnnotations;

public class RegisterRequest
{
    [Required]
    [MinLength(4, ErrorMessage = "Username must be at least 4 characters")]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must include uppercase, lowercase, number, and special character")]
    public string Password { get; set; } = null!;
}
