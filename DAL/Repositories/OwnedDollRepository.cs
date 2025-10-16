using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OwnedDollRepository : IOwnedDollRepository
    {
        private readonly DollDbContext _db;

        public OwnedDollRepository(DollDbContext db) => _db = db;

        public async Task<List<OwnedDoll>> GetAllAsync()
        {
            return await _db.OwnedDolls
                .Where(od => od.Status != "Deleted")
                .OrderByDescending(od => od.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OwnedDoll?> GetByIdAsync(int id)
        {
            return await _db.OwnedDolls
                .FirstOrDefaultAsync(od => od.OwnedDollID == id && od.Status != "Deleted");
        }

        public async Task<List<OwnedDoll>> GetByUserIdAsync(int userId)
        {
            return await _db.OwnedDolls
                .Where(od => od.UserID == userId && od.Status != "Deleted")
                .OrderByDescending(od => od.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<OwnedDoll>> GetByDollVariantIdAsync(int dollVariantId)
        {
            return await _db.OwnedDolls
                .Where(od => od.DollVariantID == dollVariantId && od.Status != "Deleted")
                .OrderByDescending(od => od.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(OwnedDoll entity)
        {
            _db.OwnedDolls.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(OwnedDoll entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.OwnedDolls.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _db.OwnedDolls.FindAsync(id);
            if (entity != null)
            {
                entity.Status = "Deleted";
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var entity = await _db.OwnedDolls.FindAsync(id);
            if (entity != null)
            {
                _db.OwnedDolls.Remove(entity);
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