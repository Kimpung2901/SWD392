using System;
using DAL.Enum;

namespace DAL.Models;

public partial class DollCharacterLink
{
    public int LinkID { get; set; }

    public int OwnedDollID { get; set; }

    public int UserCharacterID { get; set; }

    public DateTime BoundAt { get; set; }

    public DateTime? UnBoundAt { get; set; }

    public bool IsActive { get; set; }

    public string Note { get; set; } = string.Empty;

    // ✅ Chuyển sang enum
    public DollCharacterLinkStatus Status { get; set; } = DollCharacterLinkStatus.Bound; // ✅ ĐỔI
}
