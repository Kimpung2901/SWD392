using DAL.Models;

namespace DAL.IRepo
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SoftDeleteAsync(int id);
        Task HardDeleteAsync(int id);
    }
}
