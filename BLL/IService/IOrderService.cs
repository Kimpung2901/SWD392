using BLL.DTO.OrderDTO;

namespace BLL.IService
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id);
        Task<List<OrderDto>> GetByUserIdAsync(int userId);
        Task<OrderDto?> GetByIdWithItemsAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task<OrderDto?> UpdatePartialAsync(int id, UpdateOrderDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> CancelOrderAsync(int id);
    }
}