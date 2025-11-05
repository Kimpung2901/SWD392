using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OrderDTO
{
    public class CreateOrderDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public int DollVariantID { get; set; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public decimal TotalAmount { get; set; }
    }
}
