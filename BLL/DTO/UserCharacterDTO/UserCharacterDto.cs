using DAL.Enum;

namespace BLL.DTO.UserCharacterDTO
{
    public class UserCharacterDto
    {
        public int UserCharacterID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int CharacterID { get; set; }
        public string? CharacterName { get; set; }
        public int PackageId { get; set; }
        public string? PackageName { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public bool AutoRenew { get; set; }
        public UserCharacterStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public DateTime CreatedAt { get; set; }
    }
}