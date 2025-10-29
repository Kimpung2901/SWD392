using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "UserName là b?t bu?c")]
        [StringLength(255, ErrorMessage = "UserName không du?c vu?t quá 255 ký t?")]
        public string UserName { get; set; } = null!;
        [StringLength(255)]
        public string? FullName { get; set; }

        
        [StringLength(255, ErrorMessage = "Phones không du?c vu?t quá 255 ký t?")]
        public string? Phones { get; set; }
        
        [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [StringLength(255, ErrorMessage = "Email không du?c vu?t quá 255 ký t?")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Password là b?t bu?c")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password ph?i t? 6-255 ký t?")]
        public string Password { get; set; } = null!;

        [Range(1, 150, ErrorMessage = "Age ph?i t? 1 d?n 150")]
        public int? Age { get; set; }
        
        [StringLength(50, ErrorMessage = "Role không du?c vu?t quá 50 ký t?")]
        public string Role { get; set; } = "customer";
    }
}
