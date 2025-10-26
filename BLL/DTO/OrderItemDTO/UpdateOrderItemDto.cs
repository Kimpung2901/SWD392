using DAL.Enum;

namespace BLL.DTO.OrderDTO
{
    public class UpdateOrderItemDto
    {
        public OrderItemStatus? Status { get; set; }
    }
}