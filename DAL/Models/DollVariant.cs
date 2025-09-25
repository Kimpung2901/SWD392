using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class DollVariant
{
    public int DollVariantID { get; set; }

    public int DollModelID { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }
    public string Color { get; set; } = null!;
    public string Size { get; set; } = null!;

    public int SizeID { get; set; }

    public int ColorID { get; set; }

    public string Image { get; set; } = null!;

    public bool IsActive { get; set; }
}
