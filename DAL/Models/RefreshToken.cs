using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class RefreshToken
    {
        public int RefreshTokenID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedByIp { get; set; }

        public virtual User User { get; set; } = null!;
    }

}
