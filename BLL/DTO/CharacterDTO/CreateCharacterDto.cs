using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterDTO;

public class CreateCharacterDto
{
    [Required(ErrorMessage = "Name là bắt buộc")]
    [MaxLength(255, ErrorMessage = "Name không được vượt quá 255 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image là bắt buộc")]
    [MaxLength(500, ErrorMessage = "Image URL không được vượt quá 500 ký tự")]
    public string Image { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "AI URL không được vượt quá 500 ký tự")]
    [Url(ErrorMessage = "AI URL phải là URL hợp lệ")]
    public string? AIUrl { get; set; }


    [Range(0, 100, ErrorMessage = "AgeRange phải từ 0 đến 100")]
    public int AgeRange { get; set; }

    [Required(ErrorMessage = "Personality là bắt buộc")]
    [MaxLength(255)]
    public string Personality { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Description { get; set; }
}