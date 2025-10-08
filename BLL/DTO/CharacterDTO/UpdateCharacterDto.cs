namespace BLL.DTO.CharacterDTO
{
    public class UpdateCharacterDto
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Language { get; set; }
        public int? AgeRange { get; set; }
        public string? Personality { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}