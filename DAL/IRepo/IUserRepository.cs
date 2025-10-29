using DAL.Models;

namespace DAL.IRepo;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    
    IQueryable<User> GetQueryable();
    
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User entity);
    Task UpdateAsync(User entity);
    Task SoftDeleteAsync(int id);
    Task HardDeleteAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CheckUserExistsAsync(string username, string email);
}
