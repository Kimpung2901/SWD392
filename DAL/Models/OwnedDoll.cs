using System;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enum;

namespace DAL.Models;

public partial class OwnedDoll
{
    public int OwnedDollID { get; set; }

    public int UserID { get; set; }

    public int DollVariantID { get; set; }

    public string SerialCode { get; set; } = null!;

    // ✅ Chuyển sang enum
    public OwnedDollStatus Status { get; set; } = OwnedDollStatus.Active;

    public DateTime Acquired_at { get; set; }

    public DateTime Expired_at { get; set; }

    // Navigation property
    [ForeignKey(nameof(DollVariantID))]
    public virtual DollVariant? DollVariant { get; set; }
}
