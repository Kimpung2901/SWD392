using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class DollType
{
    public int DollTypeID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Create_at { get; set; }

    public string Image { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }
}
