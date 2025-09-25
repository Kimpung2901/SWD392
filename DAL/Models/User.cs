using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int UserID { get; set; }

    public string UserName { get; set; } = null!;

    public string Phones { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Role { get; set; } = null!;
    
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    virtual public ICollection<PasswordReset> PasswordResets { get; set; } = new HashSet<PasswordReset>();
}
