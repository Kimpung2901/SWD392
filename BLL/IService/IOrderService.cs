using BLL.DTO.Common;
using BLL.DTO.OrderDTO;

namespace BLL.IService;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();

    Task<PagedResult<OrderDto>> GetAsync(
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize);

    Task<PagedResult<OrderDto>> GetOrdersByUserIdAsync(
        int userId,
        string? search,
        string? sortBy,
        string? sortDir,
        int page,
        int pageSize);

    Task<OrderDto?> GetByIdAsync(int id);
    Task<List<OrderDto>> GetByUserIdAsync(int userId);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdatePartialAsync(int id, UpdateOrderDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> CancelOrderAsync(int id);
}
