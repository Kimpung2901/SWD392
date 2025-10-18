using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OwnedDollDTO
{
    public class CreateOwnedDollDto
    {
        [Required(ErrorMessage = "UserID là bắt buộc")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "DollVariantID là bắt buộc")]
        public int DollVariantID { get; set; }

        [Required(ErrorMessage = "SerialCode là bắt buộc")]
        [StringLength(255, ErrorMessage = "SerialCode không được vượt quá 255 ký tự")]
        public string SerialCode { get; set; } = null!;

        [Required(ErrorMessage = "Status là bắt buộc")]
        [StringLength(50, ErrorMessage = "Status không được vượt quá 50 ký tự")]
        public string Status { get; set; } = "Active";

        public DateTime? Acquired_at { get; set; }

        public DateTime? Expired_at { get; set; }
    }
}