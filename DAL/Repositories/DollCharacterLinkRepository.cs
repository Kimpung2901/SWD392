using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class DollCharacterLinkRepository : IDollCharacterLinkRepository
{
    private readonly DollDbContext _db;

    public DollCharacterLinkRepository(DollDbContext db) => _db = db;

    public async Task<List<DollCharacterLink>> GetAllAsync()
    {
        return await _db.DollCharacterLinks
            .OrderByDescending(l => l.BoundAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DollCharacterLink?> GetByIdAsync(int id)
    {
        return await _db.DollCharacterLinks
            .FirstOrDefaultAsync(l => l.LinkID == id);
    }

    public async Task<List<DollCharacterLink>> GetByOwnedDollIdAsync(int ownedDollId)
    {
        return await _db.DollCharacterLinks
            .Where(l => l.OwnedDollID == ownedDollId)
            .OrderByDescending(l => l.BoundAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<DollCharacterLink>> GetByUserCharacterIdAsync(int userCharacterId)
    {
        return await _db.DollCharacterLinks
            .Where(l => l.UserCharacterID == userCharacterId)
            .OrderByDescending(l => l.BoundAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<DollCharacterLink>> GetActiveLinksAsync()
    {
        return await _db.DollCharacterLinks
            .Where(l => l.IsActive && l.Status == "Active")
            .OrderByDescending(l => l.BoundAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DollCharacterLink?> GetActiveLinkByOwnedDollIdAsync(int ownedDollId)
    {
        return await _db.DollCharacterLinks
            .Where(l => l.OwnedDollID == ownedDollId && l.IsActive && l.Status == "Active")
            .OrderByDescending(l => l.BoundAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(DollCharacterLink entity)
    {
        _db.DollCharacterLinks.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(DollCharacterLink entity)
    {
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _db.DollCharacterLinks.Attach(entity);
        }

        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.DollCharacterLinks.FindAsync(id);
        if (entity != null)
        {
            _db.DollCharacterLinks.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync() > 0;
    }
}