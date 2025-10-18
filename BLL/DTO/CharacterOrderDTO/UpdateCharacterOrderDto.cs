using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CharacterOrderDTO
{
    public class UpdateCharacterOrderDto
    {
        public DateTime? Start_Date { get; set; }

        public DateTime? End_Date { get; set; }

        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string? Status { get; set; }
    }
}