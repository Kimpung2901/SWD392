using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Character
{
  

    public int CharacterId { get; set; }

    public string Name { get; set; } = null!;

    public int AgeRange { get; set; }
    public string Image { get; set; } = null!;
    public string Personality { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? AIUrl { get; set; }
}
