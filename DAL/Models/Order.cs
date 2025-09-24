using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public int UserID { get; set; }

    public int PaymentID { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public decimal ShippingAddress { get; set; }

    public string Status { get; set; } = null!;
}
