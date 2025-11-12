using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO;

public class CreatePaymentRequest : IValidatableObject
{
    [Range(typeof(decimal), "10000", "79228162514264337593543950335", 
        ErrorMessage = "Số tiền phải từ 10,000 VND trở lên.")]
    public decimal Amount { get; set; }

    public int? OrderId { get; set; }
    public int? CharacterOrderId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!OrderId.HasValue && !CharacterOrderId.HasValue)
            yield return new ValidationResult(
                "Phải cung cấp OrderId hoặc CharacterOrderId.",
                new[] { nameof(OrderId), nameof(CharacterOrderId) });

        if (OrderId.HasValue && CharacterOrderId.HasValue)
            yield return new ValidationResult(
                "Chỉ được chọn một trong OrderId hoặc CharacterOrderId.",
                new[] { nameof(OrderId), nameof(CharacterOrderId) });
    }
}

