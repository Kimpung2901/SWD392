using DAL.Models;

namespace DAL.IRepo
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(int id);
        Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
        Task AddAsync(OrderItem entity);
        Task AddRangeAsync(List<OrderItem> entities);
        Task UpdateAsync(OrderItem entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}