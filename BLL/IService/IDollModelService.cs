using BLL.DTO.Common;
using BLL.DTO.DollModelDTO;

namespace BLL.IService
{
    public interface IDollModelService
    {
        Task<PagedResult<DollModelDto>> GetAsync(
            int? dollTypeId,
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize);
        Task<DollModelDto?> GetByIdAsync(int id);
        Task<DollModelDto> CreateAsync(CreateDollModelDto dto);
        Task<DollModelDto?> UpdatePartialAsync(int id, UpdateDollModelDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}

