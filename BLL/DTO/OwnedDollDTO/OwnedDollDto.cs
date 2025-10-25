using DAL.Enum;

namespace BLL.DTO.OwnedDollDTO
{
    public class OwnedDollDto
    {
        public int OwnedDollID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int DollVariantID { get; set; }
        public string? DollVariantName { get; set; }
        public string SerialCode { get; set; } = null!;
        public OwnedDollStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public DateTime Acquired_at { get; set; }
        public DateTime Expired_at { get; set; }
    }
}