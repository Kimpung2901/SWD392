using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OwnedDollDTO
{
    public class CreateOwnedDollDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int DollVariantID { get; set; }

        [Required]
        [StringLength(255)]
        public string SerialCode { get; set; } = null!;

        [Required]
        public DateTime Acquired_at { get; set; }

        [Required]
        public DateTime Expired_at { get; set; }
    }
}