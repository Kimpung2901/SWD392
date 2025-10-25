using System;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enum;

namespace DAL.Models;

public partial class UserCharacter
{
    public int UserCharacterID { get; set; }

    public int UserID { get; set; }

    public int CharacterID { get; set; }

    public int PackageId { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public bool AutoRenew { get; set; }

    // ✅ Chuyển sang enum
    public UserCharacterStatus Status { get; set; } = UserCharacterStatus.Active;

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(UserID))]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(CharacterID))]
    public virtual Character? Character { get; set; }

    [ForeignKey(nameof(PackageId))]
    public virtual CharacterPackage? Package { get; set; }
}
