using System.ComponentModel.DataAnnotations;


namespace BLL.DTO
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;
        [StringLength(255)]
        public string? FullName { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        public string? Phones { get; set; }

        [Range(1, 150, ErrorMessage = "Age ph?i t? 1 d?n 150")]
        public int? Age { get; set; }
    }
}
