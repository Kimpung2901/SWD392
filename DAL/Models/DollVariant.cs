using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public partial class DollVariant
{
    public int DollVariantID { get; set; }

    public int DollModelID { get; set; }

    [ForeignKey(nameof(DollModelID))]
    public DollModel? DollModel { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }
    public string Color { get; set; } = null!;
    public string Size { get; set; } = null!;
    public string Image { get; set; } = null!;

    public bool IsActive { get; set; }
}
