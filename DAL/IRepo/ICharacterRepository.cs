using DAL.Models;

namespace DAL.IRepo
{
    public interface ICharacterRepository
    {
        Task<List<Character>> GetAllAsync();
        Task<Character?> GetByIdAsync(int id);
        Task AddAsync(Character entity);
        Task UpdateAsync(Character entity);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
        IQueryable<Character> Query();
    }
}
