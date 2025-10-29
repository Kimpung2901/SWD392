using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OwnedDollRepository : IOwnedDollRepository
    {
        private readonly DollDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public OwnedDollRepository(DollDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<OwnedDoll>> GetAllAsync()
        {
            return await _db.OwnedDolls
                .OrderByDescending(o => o.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OwnedDoll?> GetByIdAsync(int id)
        {
            return await _db.OwnedDolls
                .FirstOrDefaultAsync(o => o.OwnedDollID == id);
        }

        public async Task<List<OwnedDoll>> GetByUserIdAsync(int userId)
        {
            return await _db.OwnedDolls
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<OwnedDoll>> GetByDollVariantIdAsync(int dollVariantId)
        {
            return await _db.OwnedDolls
                .Where(o => o.DollVariantID == dollVariantId)
                .OrderByDescending(o => o.Acquired_at)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<OwnedDoll?> GetBySerialCodeAsync(string serialCode)
        {
            return await _db.OwnedDolls
                .FirstOrDefaultAsync(o => o.SerialCode == serialCode);
        }

        public async Task AddAsync(OwnedDoll entity)
        {
            _db.OwnedDolls.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(OwnedDoll entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.OwnedDolls.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.OwnedDolls.FindAsync(id);
            if (entity != null)
            {
                _db.OwnedDolls.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
