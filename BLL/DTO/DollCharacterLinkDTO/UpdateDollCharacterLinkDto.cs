using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.DollCharacterLinkDTO;

public class UpdateDollCharacterLinkDto
{
    [MaxLength(255, ErrorMessage = "Note không được vượt quá 255 ký tự")]
    public string? Note { get; set; }

    [MaxLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
    public string? Status { get; set; }

    public bool? IsActive { get; set; }
}