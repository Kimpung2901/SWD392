using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CharacterOrder
{
    public int CharacterOrderID { get; set; }

    public int PackageID { get; set; }

    public int CharacterID { get; set; }

    public int UserCharacterID { get; set; }

    public int QuantityMonths { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime start_date { get; set; }

    public DateTime end_date { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;
}
