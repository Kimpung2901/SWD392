using System.ComponentModel.DataAnnotations;


namespace BLL.DTO
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        public string? Phones { get; set; }

        [Range(1, 150, ErrorMessage = "Age phải từ 1 đến 150")]
        public int? Age { get; set; }
    }
}
