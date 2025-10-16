namespace BLL.DTO.DollCharacterLinkDTO
{
    public class UpdateDollCharacterLinkDto
    {
        public DateTime? BoundAt { get; set; }
        public DateTime? UnBoundAt { get; set; }
        public bool? IsActive { get; set; }
        public string? Note { get; set; }
        public string? Status { get; set; }
    }
}