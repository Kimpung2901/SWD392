using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class UpdateOrderItemDto
    {
        public int? Quantity { get; set; }
        public OrderItemStatus? Status { get; set; }
    }
}