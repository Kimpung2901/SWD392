using System.ComponentModel.DataAnnotations;
using DAL.Enum;

namespace BLL.DTO.OwnedDollDTO
{
    public class UpdateOwnedDollDto
    {
        [StringLength(255, ErrorMessage = "SerialCode không được vượt quá 255 ký tự")]
        public string? SerialCode { get; set; }

        public OwnedDollStatus? Status { get; set; }
        
        public DateTime? Expired_at { get; set; }
    }
}