using DAL.Models;

namespace DAL.IRepo
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<Order?> GetByIdWithItemsAsync(int id);
        Task AddAsync(Order entity);
        Task UpdateAsync(Order entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}