using System;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enum;

namespace DAL.Models;

public partial class CharacterOrder
{
    public int CharacterOrderID { get; set; }

    public int PackageID { get; set; }

    public int CharacterID { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public CharacterOrderStatus Status { get; set; } = CharacterOrderStatus.Pending;

    // ✅ Navigation properties
    [ForeignKey(nameof(PackageID))]
    public virtual CharacterPackage? Package { get; set; }

    [ForeignKey(nameof(CharacterID))]
    public virtual Character? Character { get; set; }

}
