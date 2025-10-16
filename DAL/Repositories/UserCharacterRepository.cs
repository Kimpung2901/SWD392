using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserCharacterRepository : IUserCharacterRepository
    {
        private readonly DollDbContext _db;

        public UserCharacterRepository(DollDbContext db) => _db = db;

        public async Task<List<UserCharacter>> GetAllAsync()
        {
            return await _db.UserCharacters
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserCharacter?> GetByIdAsync(int id)
        {
            return await _db.UserCharacters
                .FirstOrDefaultAsync(uc => uc.UserCharacterID == id);
        }

        public async Task<List<UserCharacter>> GetByUserIdAsync(int userId)
        {
            return await _db.UserCharacters
                .Where(uc => uc.UserID == userId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCharacter>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.UserCharacters
                .Where(uc => uc.CharacterID == characterId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCharacter>> GetByPackageIdAsync(int packageId)
        {
            return await _db.UserCharacters
                .Where(uc => uc.PackageId == packageId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserCharacter?> GetActiveSubscriptionAsync(int userId, int characterId)
        {
            return await _db.UserCharacters
                .Where(uc => uc.UserID == userId 
                    && uc.CharacterID == characterId 
                    && uc.Status == "Active"
                    && uc.EndAt > DateTime.UtcNow)
                .OrderByDescending(uc => uc.EndAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(UserCharacter entity)
        {
            _db.UserCharacters.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserCharacter entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.UserCharacters.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.UserCharacters.FindAsync(id);
            if (entity != null)
            {
                _db.UserCharacters.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}