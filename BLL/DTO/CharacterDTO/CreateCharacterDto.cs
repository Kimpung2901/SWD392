using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterDTO
{
    public class CreateCharacterDto
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Gender { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Language { get; set; } = null!;

        [Range(0, 150)]
        public int AgeRange { get; set; }

        [StringLength(255)]
        public string Personality { get; set; } = null!;

        [StringLength(255)]
        public string Description { get; set; } = null!;
    }
}