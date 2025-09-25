namespace DAL.Models
{
    public class PasswordReset
    {
        public int PasswordResetsID { get; set; }
        public int UserID { get; set; }
        public string Code { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool Used { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedByIp { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
