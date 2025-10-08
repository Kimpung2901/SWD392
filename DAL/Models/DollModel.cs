using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models;

public partial class DollModel
{
    [Key]
    public int DollModelID { get; set; }

    // Foreign Key
    public int DollTypeID { get; set; }

    // Navigation property (liên kết tới DollType)
    [ForeignKey(nameof(DollTypeID))]
    public DollType DollType { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Create_at { get; set; }

    public string Image { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }
}
