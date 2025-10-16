using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CharacterOrder
{
    public int CharacterOrderID { get; set; }

    public int PackageID { get; set; }

    public int CharacterID { get; set; }

    public int UserCharacterID { get; set; }

    public decimal UnitPrice { get; set; }

    public int QuantityMonths { get; set; } 

    public DateTime Start_Date { get; set; }

    public DateTime End_Date { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
