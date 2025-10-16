using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterOrderDTO
{
    public class CreateCharacterOrderDto
    {
        [Required]
        public int PackageID { get; set; }

        [Required]
        public int CharacterID { get; set; }

        [Required]
        public int UserCharacterID { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, 120)] 
        public int QuantityMonths { get; set; } 

        [Required]
        public DateTime Start_Date { get; set; }

        [Required]
        public DateTime End_Date { get; set; }
    }
}