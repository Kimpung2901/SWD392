using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public int? UserID { get; set; }  // ✅ Thay đổi thành nullable

    public int? PaymentID { get; set; }  // ✅ Thay đổi thành nullable

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Currency { get; set; }  // ✅ Thay đổi thành nullable

    public string? ShippingAddress { get; set; }  // ✅ Thay đổi thành nullable

    public string? Status { get; set; }  // ✅ Thay đổi thành nullable
    
    // ✅ Thêm navigation property để EF Core hiểu rõ relationship
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
