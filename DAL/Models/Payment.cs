using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enum;

namespace DAL.Models;

[Table("Payment")]
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentID { get; set; }

    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Method { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "VND";

    // ✅ Chuyển sang enum
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [MaxLength(100)]
    public string? TransactionId { get; set; }

    // ✅ Đổi tên: OrderId từ MoMo
    [MaxLength(100)]
    [Column("MoMoOrderId")]
    public string? OrderId { get; set; }

    [MaxLength(500)]
    public string? PayUrl { get; set; }

    [MaxLength(255)]
    public string? OrderInfo { get; set; }

    [Column(TypeName = "nvarchar(max)")]
    public string? RawResponse { get; set; }

    [Required]
    [MaxLength(50)]
    public string Target_Type { get; set; } = string.Empty;

    public int Target_Id { get; set; }

    // ✅ FK đến bảng Order
    public int? OrderID { get; set; }

    public int? CharacterOrderID { get; set; }

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime? CompletedAt { get; set; }
}
