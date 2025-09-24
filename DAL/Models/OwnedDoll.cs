using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class OwnedDoll
{
    public int OwnedDollID { get; set; }

    public int UserID { get; set; }

    public int DollVariantID { get; set; }

    public string SerialCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime acquired_at { get; set; }

    public DateTime expired_at { get; set; }
}
