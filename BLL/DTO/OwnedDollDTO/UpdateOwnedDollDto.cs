namespace BLL.DTO.OwnedDollDTO
{
    public class UpdateOwnedDollDto
    {
        public string? SerialCode { get; set; }
        public string? Status { get; set; }
        public DateTime? Acquired_at { get; set; }
        public DateTime? Expired_at { get; set; }
    }
}