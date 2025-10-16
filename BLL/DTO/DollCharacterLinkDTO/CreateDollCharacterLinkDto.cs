using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.DollCharacterLinkDTO
{
    public class CreateDollCharacterLinkDto
    {
        [Required]
        public int OwnedDollID { get; set; }

        [Required]
        public int UserCharacterID { get; set; }

        [Required]
        public DateTime BoundAt { get; set; }

        [Required]
        public DateTime UnBoundAt { get; set; }

        [Required]
        [StringLength(255)]
        public string Note { get; set; } = null!;
    }
}