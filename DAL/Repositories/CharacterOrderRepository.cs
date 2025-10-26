using DAL.IRepo;
using DAL.Models;
using DAL.Enum;  // ✅ THÊM DÒNG NÀY
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CharacterOrderRepository : ICharacterOrderRepository
    {
        private readonly DollDbContext _db;

        public CharacterOrderRepository(DollDbContext db) => _db = db;

        public async Task<List<CharacterOrder>> GetAllAsync()
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacterOrder?> GetByIdAsync(int id)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .FirstOrDefaultAsync(co => co.CharacterOrderID == id);
        }

        public async Task<List<CharacterOrder>> GetByUserCharacterIdAsync(int userCharacterId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacterOrder>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Where(co => co.CharacterID == characterId)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacterOrder>> GetByPackageIdAsync(int packageId)
        {
            return await _db.CharacterOrders
                .Include(co => co.Character)
                .Where(co => co.PackageID == packageId)
                .OrderByDescending(co => co.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<CharacterOrder>> GetPendingOrdersAsync()
        {
            return await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .Where(co => co.Status == CharacterOrderStatus.Pending)
                .OrderBy(co => co.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(CharacterOrder entity)
        {
            _db.CharacterOrders.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CharacterOrder entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.CharacterOrders.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.CharacterOrders.FindAsync(id);
            if (entity != null)
            {
                _db.CharacterOrders.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }
    }
}