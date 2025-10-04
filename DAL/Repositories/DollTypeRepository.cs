// DAL/Repositories/DollTypeRepository.cs
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class DollTypeRepository: IDollTypeRepository
    {
        private readonly DollDbContext _db;
        public DollTypeRepository(DollDbContext db) => _db = db;

        public async Task<List<DollType>> GetAllAsync(bool includeDeleted = false)
        {
            var q = _db.DollTypes.AsQueryable();
            if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
            return await q.ToListAsync();
        }

        public async Task<DollType?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var q = _db.DollTypes.AsQueryable();
            if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
            return await q.FirstOrDefaultAsync(x => x.DollTypeID == id);
        }

        public async Task AddAsync(DollType entity)
        {
            _db.DollTypes.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(DollType entity)
        {
            _db.DollTypes.Update(entity);
            await _db.SaveChangesAsync();
        }

        /// Soft delete
        public async Task<DollType?> SoftDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return null;
            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            return entity;
        }

        /// Restore a soft-deleted record
        public async Task<DollType?> RestoreAsync(int id)
        {
            var entity = await GetByIdAsync(id, includeDeleted: true);
            if (entity == null || !entity.IsDeleted) return null;
            entity.IsDeleted = false;
            await _db.SaveChangesAsync();
            return entity;
        }

        /// Hard delete (remove row from DB)
        public async Task<DollType?> HardDeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id, includeDeleted: true);
            if (entity == null) return null;

            _db.DollTypes.Remove(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
