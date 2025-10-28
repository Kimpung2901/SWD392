using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class UpdateUserDto
    {
        [StringLength(255)]
        public string? FullName { get; set; }

        
        [StringLength(255, ErrorMessage = "Phones không du?c vu?t quá 255 ký t?")]
        public string? Phones { get; set; }
        
        [EmailAddress(ErrorMessage = "Email không h?p l?")]
        [StringLength(255, ErrorMessage = "Email không du?c vu?t quá 255 ký t?")]
        public string? Email { get; set; }

        [Range(1, 150, ErrorMessage = "Age ph?i t? 1 d?n 150")]
        public int? Age { get; set; }
        
        
    }
}
