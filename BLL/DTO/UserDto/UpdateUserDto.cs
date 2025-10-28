using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class UpdateUserDto
    {
        [StringLength(255)]
        public string? FullName { get; set; }

        
        [StringLength(255, ErrorMessage = "Phones kh�ng du?c vu?t qu� 255 k� t?")]
        public string? Phones { get; set; }
        
        [EmailAddress(ErrorMessage = "Email kh�ng h?p l?")]
        [StringLength(255, ErrorMessage = "Email kh�ng du?c vu?t qu� 255 k� t?")]
        public string? Email { get; set; }

        [Range(1, 150, ErrorMessage = "Age ph?i t? 1 d?n 150")]
        public int? Age { get; set; }
        
        
    }
}
