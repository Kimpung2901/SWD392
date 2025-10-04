using BLL.DTO.DollTypeDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IDollTypeService
    {
        Task<List<DollTypeDto>> GetAllAsync();
        Task<DollTypeDto?> GetByIdAsync(int id);
        Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto);
        Task<DollTypeDto?> UpdateAsync(int id, UpdateDollTypeDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> HardDeleteAsync(int id);
    }
}
