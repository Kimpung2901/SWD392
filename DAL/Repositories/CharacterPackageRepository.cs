using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CharacterPackageRepository : ICharacterPackageRepository
    {
        private readonly DollDbContext _db;

        public CharacterPackageRepository(DollDbContext db) => _db = db;

        public async Task<List<CharacterPackage>> GetAllAsync()
        {
            return await _db.CharacterPackages
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacterPackage?> GetByIdAsync(int id)
        {
            return await _db.CharacterPackages
                .FirstOrDefaultAsync(p => p.PackageId == id && p.IsActive);
        }

        public async Task<List<CharacterPackage>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.CharacterPackages
                .Where(p => p.CharacterId == characterId && p.IsActive)
                .OrderBy(p => p.Price)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(CharacterPackage entity)
        {
            _db.CharacterPackages.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CharacterPackage entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.CharacterPackages.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _db.CharacterPackages.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false;
                entity.Status = "Deleted";
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var entity = await _db.CharacterPackages.FindAsync(id);
            if (entity != null)
            {
                _db.CharacterPackages.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}