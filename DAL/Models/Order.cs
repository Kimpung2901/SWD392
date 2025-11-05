using System;
using DAL.Enum;

namespace DAL.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public int? UserID { get; set; }

    public int? PaymentID { get; set; }

    public int DollVariantID { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public virtual DollVariant? DollVariant { get; set; }
}
