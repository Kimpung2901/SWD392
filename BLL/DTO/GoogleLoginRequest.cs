using System.ComponentModel.DataAnnotations;

namespace BLL.DTO
{
    public class GoogleLoginRequest
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
