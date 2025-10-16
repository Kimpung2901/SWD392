using BLL.DTO.OwnedDollDTO;

namespace BLL.IService
{
    public interface IOwnedDollService
    {
        Task<List<OwnedDollDto>> GetAllAsync();
        Task<OwnedDollDto?> GetByIdAsync(int id);
        Task<List<OwnedDollDto>> GetByUserIdAsync(int userId);
        Task<List<OwnedDollDto>> GetByDollVariantIdAsync(int dollVariantId);
        Task<OwnedDollDto> CreateAsync(CreateOwnedDollDto dto);
        Task<OwnedDollDto?> UpdatePartialAsync(int id, UpdateOwnedDollDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}