using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class UpdateOrderDto
    {
        public string? ShippingAddress { get; set; }
        public OrderStatus? Status { get; set; }
    }
}