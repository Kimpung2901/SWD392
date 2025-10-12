
using DAL.Models;
namespace DAL.IRepo
{
    public interface ICharacterOrderRepository
    {
        Task<List<CharacterOrder>> GetAllAsync();
        Task<CharacterOrder?> GetByIdAsync(int id);
        Task<CharacterOrder> CreateAsync(CharacterOrder dto);
        Task<CharacterOrder?> UpdateAsync(int id, CharacterOrder dto);
        Task<bool> DeleteAsync(int id);

    }
}