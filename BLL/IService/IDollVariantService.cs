using BLL.DTO.Common;
using BLL.DTO.DollVariantDTO;

namespace BLL.IService
{
    public interface IDollVariantService
    {
        Task<PagedResult<DollVariantDto>> GetAsync(
            int? dollModelId,
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize);
        Task<DollVariantDto?> GetByIdAsync(int id);
        Task<DollVariantDto> CreateAsync(CreateDollVariantDto dto);
        Task<DollVariantDto?> UpdatePartialAsync(int id, UpdateDollVariantDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
