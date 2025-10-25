using System;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enum;

namespace DAL.Models;

public partial class OrderItem
{                                   
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int DollVariantID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    
    // ✅ Chuyển sang enum
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;
    
    // Navigation properties
    public virtual Order? Order { get; set; }
    public virtual DollVariant? DollVariant { get; set; }
}
