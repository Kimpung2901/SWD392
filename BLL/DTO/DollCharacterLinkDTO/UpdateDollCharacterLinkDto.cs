using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.DollCharacterLinkDTO;

public class UpdateDollCharacterLinkDto
{
    [MaxLength(255, ErrorMessage = "Note không được vượt quá 255 ký tự")]
    public string? Note { get; set; }

    public DollCharacterLinkStatus? Status { get; set; }

    public bool? IsActive { get; set; }
}