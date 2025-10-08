using System.ComponentModel.DataAnnotations;
namespace BLL.DTO
{
    public class ForgotPasswordOtpRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}
