using BLL.DTO.CharacterOrderDTO;

namespace BLL.IService
{
    public interface ICharacterOrderService
    {
        Task<List<CharacterOrderRequest>> GetAllAsync();
        Task<CharacterOrderRequest?> GetByIdAsync(int id);
        Task<CharacterOrderRequest> CreateAsync(CharacterOrderRequest dto);
        Task<CharacterOrderRequest?> UpdateAsync(int id, CharacterOrderRequest dto);
        Task<bool> DeleteAsync(int id);
    }
}