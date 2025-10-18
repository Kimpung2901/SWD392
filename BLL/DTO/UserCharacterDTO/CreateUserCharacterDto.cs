using System.ComponentModel.DataAnnotations;

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

        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string Status { get; set; } = "Active";
    }
}