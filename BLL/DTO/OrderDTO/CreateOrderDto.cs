using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OrderDTO
{
    public class CreateOrderDto
    {
        [Required]
        public int DollVariantID { get; set; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;
    }
}
