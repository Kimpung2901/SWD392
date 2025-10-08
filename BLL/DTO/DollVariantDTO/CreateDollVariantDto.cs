using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.DollVariantDTO
{
    public class CreateDollVariantDto
    {
        [Required]
        public int DollModelID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; } = null!;

        [Required]
        [StringLength(5)]
        public string Size { get; set; } = null!;

        [StringLength(255)]
        public string Image { get; set; } = null!;
    }
}