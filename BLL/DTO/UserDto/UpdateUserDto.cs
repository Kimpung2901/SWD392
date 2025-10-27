using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class UpdateUserDto
    {
        [StringLength(255, ErrorMessage = "UserName không được vượt quá 255 ký tự")]
        public string? UserName { get; set; }
        
        [StringLength(255, ErrorMessage = "Phones không được vượt quá 255 ký tự")]
        public string? Phones { get; set; }
        
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string? Email { get; set; }

        [Range(1, 150, ErrorMessage = "Age phải từ 1 đến 150")]
        public int? Age { get; set; }
        
        public string? Status { get; set; }
        
        [StringLength(50, ErrorMessage = "Role không được vượt quá 50 ký tự")]
        public string? Role { get; set; }
        
        public bool? IsDeleted { get; set; }
    }
}
