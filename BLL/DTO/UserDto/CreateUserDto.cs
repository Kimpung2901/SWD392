using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserDTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "UserName l� b?t bu?c")]
        [StringLength(255, ErrorMessage = "UserName kh�ng du?c vu?t qu� 255 k� t?")]
        public string UserName { get; set; } = null!;
        [StringLength(255)]
        public string? FullName { get; set; }

        
        [StringLength(255, ErrorMessage = "Phones kh�ng du?c vu?t qu� 255 k� t?")]
        public string? Phones { get; set; }
        
        [Required(ErrorMessage = "Email l� b?t bu?c")]
        [EmailAddress(ErrorMessage = "Email kh�ng h?p l?")]
        [StringLength(255, ErrorMessage = "Email kh�ng du?c vu?t qu� 255 k� t?")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "Password l� b?t bu?c")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password ph?i t? 6-255 k� t?")]
        public string Password { get; set; } = null!;

        [Range(1, 150, ErrorMessage = "Age ph?i t? 1 d?n 150")]
        public int? Age { get; set; }
        
        [StringLength(50, ErrorMessage = "Role kh�ng du?c vu?t qu� 50 k� t?")]
        public string Role { get; set; } = "customer";
    }
}
