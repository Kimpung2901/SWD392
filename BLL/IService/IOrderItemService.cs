using BLL.DTO.OrderDTO;

namespace BLL.IService
{
    public interface IOrderItemService
    {
        Task<List<OrderItemDto>> GetAllAsync();
        Task<OrderItemDto?> GetByIdAsync(int id);
        Task<List<OrderItemDto>> GetByOrderIdAsync(int orderId);
        Task<OrderItemDto?> UpdatePartialAsync(int id, UpdateOrderItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}