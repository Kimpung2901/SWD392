using BLL.DTO.DollCharacterLinkDTO;

namespace BLL.IService;

public interface IDollCharacterLinkService
{
    Task<List<DollCharacterLinkDto>> GetAllAsync();
    Task<DollCharacterLinkDto?> GetByIdAsync(int id);
    Task<List<DollCharacterLinkDto>> GetByOwnedDollIdAsync(int ownedDollId);
    Task<List<DollCharacterLinkDto>> GetByUserCharacterIdAsync(int userCharacterId);
    Task<List<DollCharacterLinkDto>> GetActiveLinksAsync();
    Task<DollCharacterLinkDto> CreateAsync(CreateDollCharacterLinkDto dto);
    Task<DollCharacterLinkDto?> UpdatePartialAsync(int id, UpdateDollCharacterLinkDto dto);
    Task<bool> UnbindAsync(int id);
    Task<bool> DeleteAsync(int id);
}