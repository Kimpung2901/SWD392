using System;
using DAL.Enum;

namespace DAL.Models;

public partial class CharacterPackage
{
    public int PackageId { get; set; }

    public int CharacterId { get; set; }

    public string Name { get; set; } = null!;

    public string Billing_Cycle { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public CharacterPackageStatus Status { get; set; } = CharacterPackageStatus.Active;
    public int DurationDays { get; set; }
}
