using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTO;

public class CreatePaymentRequest : IValidatableObject
{
    [Required, Range(1000, double.MaxValue)]
    public decimal Amount { get; set; }

    public int? OrderId { get; set; }
    public int? CharacterOrderId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!OrderId.HasValue && !CharacterOrderId.HasValue)
            yield return new ValidationResult(
                "Phai cung cap OrderId hoac CharacterOrderId.",
                new[] { nameof(OrderId), nameof(CharacterOrderId) });

        if (OrderId.HasValue && CharacterOrderId.HasValue)
            yield return new ValidationResult(
                "Chi duoc chon mot trong OrderId hoac CharacterOrderId.",
                new[] { nameof(OrderId), nameof(CharacterOrderId) });
    }
}
