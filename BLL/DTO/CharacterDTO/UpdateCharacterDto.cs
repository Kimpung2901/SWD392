using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterDTO;

public class UpdateCharacterDto
{
    [MaxLength(255, ErrorMessage = "Name không được vượt quá 255 ký tự")]
    public string? Name { get; set; }

    [MaxLength(500, ErrorMessage = "Image URL không được vượt quá 500 ký tự")]
    public string? Image { get; set; }

    [Range(0, 100, ErrorMessage = "AgeRange phải từ 0 đến 100")]
    public int? AgeRange { get; set; }

    [MaxLength(255)]
    public string? Personality { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}