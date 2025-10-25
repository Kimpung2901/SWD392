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

        [Required]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }
}