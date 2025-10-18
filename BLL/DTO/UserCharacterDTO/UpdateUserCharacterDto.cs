using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.UserCharacterDTO
{
    public class UpdateUserCharacterDto
    {
        public DateTime? StartAt { get; set; }

        public DateTime? EndAt { get; set; }

        public bool? AutoRenew { get; set; }

        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string? Status { get; set; }
    }
}