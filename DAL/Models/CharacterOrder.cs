using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public partial class CharacterOrder
{
    public int CharacterOrderID { get; set; }

    public int PackageID { get; set; }

    public int CharacterID { get; set; }

    public int UserCharacterID { get; set; }

    public int QuantityMonths { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime Start_Date { get; set; }

    public DateTime End_Date { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    // ✅ Navigation properties
    [ForeignKey(nameof(PackageID))]
    public virtual CharacterPackage? Package { get; set; }

    [ForeignKey(nameof(CharacterID))]
    public virtual Character? Character { get; set; }

    [ForeignKey(nameof(UserCharacterID))]
    public virtual UserCharacter? UserCharacter { get; set; }
}
