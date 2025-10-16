namespace BLL.DTO.UserCharacterDTO
{
    public class UpdateUserCharacterDto
    {
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool? AutoRenew { get; set; }
        public string? Status { get; set; }
    }
}