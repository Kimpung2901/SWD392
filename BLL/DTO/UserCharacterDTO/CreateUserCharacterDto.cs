using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserCharacterDTO
{
    public class CreateUserCharacterDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int CharacterID { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        public bool AutoRenew { get; set; } = false;
    }
}