using System.ComponentModel.DataAnnotations;

namespace BLL.DTO;

public class CreatePaymentRequest
{
    [Required, Range(1000, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string TargetType { get; set; } = "Order"; // "Order" | "CharacterOrder"

    [Required]
    public int TargetId { get; set; }

    public int? OrderId { get; set; }
    public int? CharacterOrderId { get; set; }
}
