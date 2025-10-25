using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "UserName là bắt buộc")]
        [StringLength(255, ErrorMessage = "UserName không được vượt quá 255 ký tự")]
        public string UserName { get; set; } = null!;
        
        [StringLength(255, ErrorMessage = "Phones không được vượt quá 255 ký tự")]
        public string? Phones { get; set; }
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password phải từ 6-255 ký tự")]
        public string Password { get; set; } = null!;
        
        // ✅ THÊM MỚI
        [Range(1, 150, ErrorMessage = "Age phải từ 1 đến 150")]
        public int? Age { get; set; }
        
        [StringLength(50, ErrorMessage = "Role không được vượt quá 50 ký tự")]
        public string Role { get; set; } = "customer";
    }
}
