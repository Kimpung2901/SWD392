using System;
using System.Collections.Generic;
using DAL.Enum;

namespace DAL.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public int? UserID { get; set; }

    public int? PaymentID { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = null!;

    // ✅ Chuyển sang enum
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
