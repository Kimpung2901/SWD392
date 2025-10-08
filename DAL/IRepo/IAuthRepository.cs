using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.IRepo
{
    public interface IAuthRepository
    {
        DbSet<User> Users { get; }

        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> SaveChangesAsync();
    }
}
