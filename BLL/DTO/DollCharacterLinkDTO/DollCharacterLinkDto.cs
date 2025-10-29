using DAL.Enum;

namespace BLL.DTO.DollCharacterLinkDTO;

public class DollCharacterLinkDto
{
    public int LinkID { get; set; }
    public int OwnedDollID { get; set; }
    public string? OwnedDollSerialCode { get; set; }
    public int UserCharacterID { get; set; }
    public string? CharacterName { get; set; }
    public DateTime BoundAt { get; set; }
    public DateTime UnBoundAt { get; set; } 
    public bool IsActive { get; set; }
    public string Note { get; set; } = string.Empty;
    public DollCharacterLinkStatus Status { get; set; }
    public string StatusDisplay => Status.ToString();
}