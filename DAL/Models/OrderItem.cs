using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public partial class OrderItem
{                                   
    public int OrderItemID { get; set; }
    public int OrderID { get; set; }
    public int DollVariantID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string Status { get; set; } = null!;
    
    // ✅ Navigation properties KHÔNG cần [ForeignKey] attribute
    // EF Core sẽ tự nhận diện dựa trên tên property và configuration trong DbContext
    public virtual Order? Order { get; set; }
    public virtual DollVariant? DollVariant { get; set; }
}
