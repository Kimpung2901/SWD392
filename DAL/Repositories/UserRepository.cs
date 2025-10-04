using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DollDbContext _db;
        public UserRepository(DollDbContext db) => _db = db;

        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Users
                .Where(u => !u.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserID == id && !u.IsDeleted);
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
        }
    }
}
