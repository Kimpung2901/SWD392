using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class User
{
    public int UserID { get; set; }

    public string UserName { get; set; } = null!;

    public string? Phones { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Status { get; set; } = "Active";

    // ✅ Sửa default từ "user" thành "Customer"
    public string Role { get; set; } = "Customer";

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
