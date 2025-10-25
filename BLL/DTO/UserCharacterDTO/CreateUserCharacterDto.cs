using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.UserCharacterDTO
{
    public class CreateUserCharacterDto
    {
        [Required(ErrorMessage = "UserID là bắt buộc")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "CharacterID là bắt buộc")]
        public int CharacterID { get; set; }

        [Required(ErrorMessage = "PackageId là bắt buộc")]
        public int PackageId { get; set; }

        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool AutoRenew { get; set; } = false;

        public UserCharacterStatus Status { get; set; } = UserCharacterStatus.Active;
    }
}