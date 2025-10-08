using BLL.DTO.DollVariantDTO;

namespace BLL.IService
{
    public interface IDollVariantService
    {
        Task<List<DollVariantDto>> GetAllAsync();
        Task<DollVariantDto?> GetByIdAsync(int id);
        Task<List<DollVariantDto>> GetByDollModelIdAsync(int dollModelId);
        Task<DollVariantDto> CreateAsync(CreateDollVariantDto dto);
        Task<DollVariantDto?> UpdatePartialAsync(int id, UpdateDollVariantDto dto);
        Task<bool> DeleteAsync(int id);
    }
}