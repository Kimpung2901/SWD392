using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class DollModel
{
    public int DollModelID { get; set; }

    public int DollTypeID { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Create_at { get; set; }

    public string Image { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }
}
