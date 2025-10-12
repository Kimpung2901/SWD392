using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterPackageDTO
{
    
    public class CreateCharacterPackageDto
    {
        [Required]
        public int CharacterId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Billing_Cycle { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";
    }
}