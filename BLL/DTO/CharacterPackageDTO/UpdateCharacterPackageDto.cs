namespace BLL.DTO.CharacterPackageDTO
{
    public class UpdateCharacterPackageDto
    {
        public int? CharacterId { get; set; }
        public string? Name { get; set; }
        public string? Billing_Cycle { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
    }
}