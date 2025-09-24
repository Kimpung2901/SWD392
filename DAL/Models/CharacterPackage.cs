using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CharacterPackage
{
    public int PackageId { get; set; }

    public int CharacterId { get; set; }

    public string Name { get; set; } = null!;

    public int DurationDays { get; set; }

    public string billing_cycle { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;
}
