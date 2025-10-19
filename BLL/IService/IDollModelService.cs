using BLL.DTO.DollModelDTO;

namespace BLL.IService
{
    public interface IDollModelService
    {
        Task<List<DollModelDto>> GetAllAsync();
        Task<DollModelDto?> GetByIdAsync(int id);
        Task<List<DollModelDto>> GetByDollTypeIdAsync(int dollTypeId);
        Task<DollModelDto> CreateAsync(CreateDollModelDto dto);
        Task<DollModelDto?> UpdatePartialAsync(int id, UpdateDollModelDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}
