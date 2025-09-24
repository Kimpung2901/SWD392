using System;
using System.Collections.Generic;

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

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
