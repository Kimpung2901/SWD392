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
                .Where(cp => cp.IsActive)
                .OrderByDescending(cp => cp.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacterPackage?> GetByIdAsync(int id)
        {
            return await _db.CharacterPackages
                .FirstOrDefaultAsync(cp => cp.PackageId == id && cp.IsActive);
        }

        public async Task<List<CharacterPackage>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.CharacterPackages
                .Where(cp => cp.CharacterId == characterId && cp.IsActive)
                .OrderByDescending(cp => cp.CreatedAt)
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

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.CharacterPackages.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false; // Soft delete
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}