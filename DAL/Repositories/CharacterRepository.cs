using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly DollDbContext _db;

        public CharacterRepository(DollDbContext db) => _db = db;

        public async Task<List<Character>> GetAllAsync()
        {
            return await _db.Characters
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Character?> GetByIdAsync(int id)
        {
            return await _db.Characters

                .FirstOrDefaultAsync(c => c.CharacterId == id && c.IsActive);
        }

        public async Task AddAsync(Character entity)
        {
            _db.Characters.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Character entity)
        {

            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.Characters.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Characters.FindAsync(id);
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

        public IQueryable<Character> Query()
        {
            return _db.Characters.AsNoTracking();
        }
    }
}
