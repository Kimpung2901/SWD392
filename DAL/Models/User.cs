using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int UserID { get; set; }

    public string UserName { get; set; }

    public string? Phones { get; set; }

    public string Email { get; set; }

    public string Password { get; set; } = null!;

    public string Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Role { get; set; }
    
    public bool IsDeleted { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    
}
