namespace BLL.DTO.CharacterDTO
{
    public class CharacterDto
    {
        public int CharacterId { get; set; }
        public string Name { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Language { get; set; } = null!;
        public int AgeRange { get; set; }
        public string Personality { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}