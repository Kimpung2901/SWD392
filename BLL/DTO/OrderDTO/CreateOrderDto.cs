using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.OrderDTO
{
    public class CreateOrderDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(500)]
        public string ShippingAddress { get; set; } = null!;

        [StringLength(10)]
        public string Currency { get; set; } = "VND";

        [Required]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }
}