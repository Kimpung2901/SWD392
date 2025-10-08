using System.ComponentModel.DataAnnotations;
namespace BLL.DTO
{
    public class ResetPasswordOtpRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, RegularExpression(@"^\d{6}$")]
        public string Otp { get; set; } = null!;

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = null!;

        [Required, Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
