using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace DAL.Repositories
{
    public class DollTypeRepository : IDollTypeRepository
    {
        private readonly DollDbContext _db;
        public DollTypeRepository(DollDbContext db) => _db = db;

        public async Task<List<DollType>> GetAllAsync()
            => await _db.DollTypes
                        .Where(x => !x.IsDeleted)
                        .AsNoTracking()
                        .ToListAsync();

        public async Task<DollType?> GetByIdAsync(int id)
            => await _db.DollTypes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.DollTypeID == id && !x.IsDeleted);

        public async Task AddAsync(DollType entity)
        {
            _db.DollTypes.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<DollType?> UpdateAsync(DollType entity)
        {
            var existing = await _db.DollTypes.FindAsync(entity.DollTypeID);
            if (existing == null || existing.IsDeleted)
                return null;

            _db.Entry(existing).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _db.DollTypes.FindAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            var entity = await _db.DollTypes.FindAsync(id);
            if (entity == null) return false;
            _db.DollTypes.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }

        public IQueryable<DollType> Query()
        {
            return _db.DollTypes
                .AsNoTracking();
        }
    }
}
