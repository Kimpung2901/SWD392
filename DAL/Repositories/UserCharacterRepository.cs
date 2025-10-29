using DAL.IRepo;
using DAL.Models;
using DAL.Enum; 
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserCharacterRepository : IUserCharacterRepository
    {
        private readonly DollDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public UserCharacterRepository(DollDbContext db, IUnitOfWork unitOfWork)
        {
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserCharacter>> GetAllAsync()
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)  
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserCharacter?> GetByIdAsync(int id)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Include(uc => uc.Package) 
                .FirstOrDefaultAsync(uc => uc.UserCharacterID == id);
        }

        public async Task<List<UserCharacter>> GetByUserIdAsync(int userId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)  
                .Where(uc => uc.UserID == userId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCharacter>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Package) 
                .Where(uc => uc.CharacterID == characterId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCharacter>> GetByPackageIdAsync(int packageId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Where(uc => uc.PackageId == packageId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<UserCharacter>> GetActiveSubscriptionsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _db.UserCharacters
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)
                .Where(uc => uc.UserID == userId 
                    && uc.Status == UserCharacterStatus.Active 
                    && uc.EndAt > now)
                .OrderByDescending(uc => uc.EndAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(UserCharacter entity)
        {
            _db.UserCharacters.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserCharacter entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _db.UserCharacters.Attach(entity);
            }

            _db.Entry(entity).State = EntityState.Modified;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.UserCharacters.FindAsync(id);
            if (entity != null)
            {
                _db.UserCharacters.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
