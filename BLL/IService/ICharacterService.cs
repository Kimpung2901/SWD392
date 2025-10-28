using BLL.DTO.CharacterDTO;
using BLL.DTO.Common;

namespace BLL.IService
{
    public interface ICharacterService
    {
        Task<PagedResult<CharacterDto>> GetAsync(string? search, string? sortBy, string? sortDir, int page, int pageSize);
        Task<CharacterDto?> GetByIdAsync(int id);
        Task<CharacterDto> CreateAsync(CreateCharacterDto dto);
        Task<CharacterDto?> UpdatePartialAsync(int id, UpdateCharacterDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
