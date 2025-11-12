using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BLL.Validation
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("Password không được để trống");

            var password = value.ToString()!;

            if (password.Length < 8)
                return new ValidationResult("Password phải có ít nhất 8 ký tự");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return new ValidationResult("Password phải có ít nhất 1 chữ in hoa");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return new ValidationResult("Password phải có ít nhất 1 chữ thường");

            if (!Regex.IsMatch(password, @"\d"))
                return new ValidationResult("Password phải có ít nhất 1 số");

            if (!Regex.IsMatch(password, @"[@$!%*?&#]"))
                return new ValidationResult("Password phải có ít nhất 1 ký tự đặc biệt (@$!%*?&#)");

            return ValidationResult.Success;
        }
    }
}