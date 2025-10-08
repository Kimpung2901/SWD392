using BLL.DTO.CharacterDTO;

namespace BLL.IService
{
    public interface ICharacterService
    {
        Task<List<CharacterDto>> GetAllAsync();
        Task<CharacterDto?> GetByIdAsync(int id);
        Task<CharacterDto> CreateAsync(CreateCharacterDto dto);
        Task<CharacterDto?> UpdatePartialAsync(int id, UpdateCharacterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}