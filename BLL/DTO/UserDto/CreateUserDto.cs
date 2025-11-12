using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Username phải từ 3-255 ký tự")]
        public string UserName { get; set; } = null!;

        [StringLength(255, ErrorMessage = "FullName không được vượt quá 255 ký tự")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6-100 ký tự")]
        public string Password { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(255, ErrorMessage = "Số điện thoại không được vượt quá 255 ký tự")]
        public string? Phones { get; set; }

        [Range(1, 150, ErrorMessage = "Tuổi phải từ 1 đến 150")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Role là bắt buộc")]
        [RegularExpression("^(admin|manager|customer)$", ErrorMessage = "Role phải là: admin, manager, hoặc customer")]
        public string Role { get; set; } = "customer";
    }
}
