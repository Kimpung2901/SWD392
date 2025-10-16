using System.ComponentModel.DataAnnotations;

namespace BLL.DTO;

public class OrderInfoModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string OrderInfo { get; set; } = string.Empty;

    [Required]
    [Range(1000, double.MaxValue, ErrorMessage = "Amount must be at least 1000 VND")]
    public decimal Amount { get; set; }
}