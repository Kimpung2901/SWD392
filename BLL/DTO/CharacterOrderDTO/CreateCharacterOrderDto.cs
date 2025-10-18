using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterOrderDTO
{
    public class CreateCharacterOrderDto
    {
        [Required(ErrorMessage = "PackageID là bắt buộc")]
        public int PackageID { get; set; }

        [Required(ErrorMessage = "CharacterID là bắt buộc")]
        public int CharacterID { get; set; }

        [Required(ErrorMessage = "UserCharacterID là bắt buộc")]
        public int UserCharacterID { get; set; }

        [Required(ErrorMessage = "QuantityMonths là bắt buộc")]
        [Range(1, 120, ErrorMessage = "QuantityMonths phải từ 1 đến 120 tháng")]
        public int QuantityMonths { get; set; }

        public DateTime? Start_Date { get; set; }

        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string Status { get; set; } = "Pending";
    }
}