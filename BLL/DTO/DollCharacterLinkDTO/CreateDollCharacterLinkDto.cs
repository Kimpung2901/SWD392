using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.DollCharacterLinkDTO;

public class CreateDollCharacterLinkDto
{
    [Required(ErrorMessage = "OwnedDollID là bắt buộc")]
    public int OwnedDollID { get; set; }

    [Required(ErrorMessage = "UserCharacterID là bắt buộc")]
    public int UserCharacterID { get; set; }

    public DateTime? BoundAt { get; set; }

    [MaxLength(255, ErrorMessage = "Note không được vượt quá 255 ký tự")]
    public string? Note { get; set; }
}