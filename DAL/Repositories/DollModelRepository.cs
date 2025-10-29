using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DAL.Repositories
{
    public class DollModelRepository : IDollModelRepository
    {
        private readonly DollDbContext _db;
        public DollModelRepository(DollDbContext db) => _db = db;

        public async Task<List<DollModel>> GetAllAsync()
        {
            return await _db.DollModels
                .Include(m => m.DollType)
                .ToListAsync();
        }

        public async Task<DollModel?> GetByIdAsync(int id)
        {
            return await _db.DollModels
                .Include(m => m.DollType) 
                .FirstOrDefaultAsync(m => m.DollModelID == id && !m.IsDeleted);
        }

        public async Task AddAsync(DollModel entity)
        {
            _db.DollModels.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(DollModel entity)
        {
            _db.DollModels.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var model = await _db.DollModels.FindAsync(id);
            if (model != null)
            {
                model.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task HardDeleteAsync(int id)
        {
            var model = await _db.DollModels.FindAsync(id);
            if (model != null)
            {
                _db.DollModels.Remove(model);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<DollModel>> GetByTypeIdAsync(int dollTypeId)
        {
            return await _db.DollModels
                .Where(m => m.DollTypeID == dollTypeId && !m.IsDeleted)
                .Include(m => m.DollType) 
                .ToListAsync();
        }

        public IQueryable<DollModel> Query()
        {
            return _db.DollModels
                .Include(m => m.DollType)
                .AsNoTracking();
        }
    }
}
