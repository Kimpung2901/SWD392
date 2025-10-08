using DAL.Models;

namespace DAL.IRepo
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CheckUserExistsAsync(string username, string email);

        Task AddAsync(User user);
        Task<bool> SaveChangesAsync();
        Task UpdateAsync(User user);
        Task SoftDeleteAsync(int id);
        Task HardDeleteAsync(int id);
    }
}
