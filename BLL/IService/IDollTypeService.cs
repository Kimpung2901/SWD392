using BLL.DTO.Common;
using BLL.DTO.DollTypeDTO;

namespace BLL.Services
{
    public interface IDollTypeService
    {
        Task<PagedResult<DollTypeDto>> GetAsync(string? search, string? sortBy, string? sortDir, int page, int pageSize);
        Task<DollTypeDto?> GetByIdAsync(int id);
        Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto);
        Task<DollTypeDto?> UpdatePartialAsync(int id, UpdateDollTypeDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}
