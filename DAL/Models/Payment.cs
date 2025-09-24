using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Payment
{
    public int PaymentID { get; set; }

    public int CharacterOrderID { get; set; }

    public string Provider { get; set; } = null!;

    public string Method { get; set; } = null!;

    public decimal Amount { get; set; }

    public int Currency { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string target_type { get; set; } = null!;

    public int target_id { get; set; }
}
