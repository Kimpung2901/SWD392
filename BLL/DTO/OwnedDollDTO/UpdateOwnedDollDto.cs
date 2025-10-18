using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OwnedDollDTO
{
    public class UpdateOwnedDollDto
    {
        [StringLength(255, ErrorMessage = "SerialCode không được vượt quá 255 ký tự")]
        public string? SerialCode { get; set; }

        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string? Status { get; set; }
        public DateTime? Expired_at { get; set; }
    }
}