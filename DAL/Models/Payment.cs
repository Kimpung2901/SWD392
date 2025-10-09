using System;

namespace DAL.Models;

public partial class Payment
{
    public int PaymentID { get; set; }

    // chỉ một trong hai có giá trị, tùy Target_Type
    public int? OrderID { get; set; }
    public int? CharacterOrderID { get; set; }

    public string Provider { get; set; } = "Unknown";   // "MoMo" | "VNPay"
    public string Method { get; set; } = "Redirect";     // "QR" | "ATM" | "Wallet"...

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";     

    public string Status { get; set; } = "Pending";      // Pending | Success | Failed | Canceled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Target_Type { get; set; } = "Order";   // "Order" | "CharacterOrder"
    public int Target_Id { get; set; }

    public string? TransactionId { get; set; }           // MoMo: orderId/requestId ; VNPay: vnp_TxnRef
    public string? PayUrl { get; set; }                  // URL để redirect người dùng
    public string? RawResponse { get; set; }             // log payload trả về
}
