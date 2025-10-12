using DAL.IRepo;
using DAL.Models;
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
                .OrderByDescending(o => o.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CharacterOrder?> GetByIdAsync(int id)
        {
            return await _db.CharacterOrders
                .FirstOrDefaultAsync(o => o.CharacterOrderID == id);
        }

        public async Task<CharacterOrder> CreateAsync(CharacterOrder entity)
        {
            _db.CharacterOrders.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<CharacterOrder?> UpdateAsync(int id, CharacterOrder entity)
        {
            var existing = await _db.CharacterOrders.FindAsync(id);
            if (existing == null) return null;

            existing.PackageID = entity.PackageID;
            existing.CharacterID = entity.CharacterID;
            existing.UserCharacterID = entity.UserCharacterID;
            existing.UnitPrice = entity.UnitPrice;
            existing.Start_Date = entity.Start_Date;
            existing.End_Date = entity.End_Date;
            existing.Status = entity.Status;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.CharacterOrders.FindAsync(id);
            if (entity != null)
            {
                _db.CharacterOrders.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}