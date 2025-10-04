using DAL.Models;

public interface IDollTypeRepository
{
    Task<List<DollType>> GetAllAsync(bool includeDeleted = false);
    Task<DollType?> GetByIdAsync(int id, bool includeDeleted = false);
    Task AddAsync(DollType entity);
    Task UpdateAsync(DollType entity);

    // Soft Delete / Restore / Hard Delete
    Task<DollType?> SoftDeleteAsync(int id);
    Task<DollType?> RestoreAsync(int id);
    Task<DollType?> HardDeleteAsync(int id);
}