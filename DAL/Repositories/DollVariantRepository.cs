using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL.Repositories
{
    public class DollVariantRepository : IDollVariantRepository
    {
        private readonly DollDbContext _db;

        public DollVariantRepository(DollDbContext db) => _db = db;

        public async Task<List<DollVariant>> GetAllAsync()
        {
            return await _db.DollVariants
                .Include(v => v.DollModel) 
                .Where(v => v.IsActive)
                .ToListAsync();
        }

        public async Task<DollVariant?> GetByIdAsync(int id)
        {
            return await _db.DollVariants
                .Include(v => v.DollModel)
                .FirstOrDefaultAsync(v => v.DollVariantID == id && v.IsActive);
        }

        public async Task<List<DollVariant>> GetByDollModelIdAsync(int dollModelId)
        {
            return await _db.DollVariants
                .Include(v => v.DollModel) 
                .Where(v => v.DollModelID == dollModelId && v.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(DollVariant entity)
        {
            _db.DollVariants.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(DollVariant entity)
        {
            _db.DollVariants.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.DollVariants.FindAsync(id);
            if (entity != null)
            {
                entity.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public IQueryable<DollVariant> Query()
        {
            return _db.DollVariants
                .Include(v => v.DollModel)
                .AsNoTracking();
        }
    }
}
