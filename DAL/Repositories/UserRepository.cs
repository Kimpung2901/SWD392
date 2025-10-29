using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DollDbContext _db;

    public UserRepository(DollDbContext db) => _db = db;

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users.AsNoTracking().ToListAsync();
    }
    public IQueryable<User> GetQueryable()
    {
        return _db.Users.AsQueryable();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task AddAsync(User entity)
    {
        _db.Users.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        _db.Users.Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            user.IsDeleted = true;
            await UpdateAsync(user);
        }
    }

    public async Task HardDeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> CheckUserExistsAsync(string username, string email)
    {
        return await _db.Users.AnyAsync(u => u.UserName == username || u.Email == email);
    }
}
