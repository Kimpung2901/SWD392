namespace BLL.DTO.CharacterDTO;

public class CharacterDto
{
    public int CharacterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int AgeRange { get; set; }
    public string Personality { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AIUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}