using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class DollCharacterLink
{
    public int LinkID { get; set; }

    public int OwnedDollID { get; set; }

    public int UserCharacterID { get; set; }

    public DateTime BoundAt { get; set; }

    public DateTime UnBoundAtbigint { get; set; }

    public bool IsActive { get; set; }

    public string Note { get; set; } = null!;

    public string Status { get; set; } = null!;
}
