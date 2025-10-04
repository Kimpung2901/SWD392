using BLL.DTO.DollTypeDTO;

public interface IDollTypeService
{
    Task<List<DollTypeDto>> GetAllAsync(bool includeDeleted = false);
    Task<DollTypeDto?> GetByIdAsync(int id, bool includeDeleted = false);
    Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto);
    Task<DollTypeDto?> UpdateAsync(int id, string name, string description, string image, bool isActive);

    // Soft Delete / Restore / Hard Delete
    Task<DollTypeDto?> DeleteSoftAsync(int id);
    Task<DollTypeDto?> RestoreAsync(int id);
    Task<DollTypeDto?> DeleteHardAsync(int id);
}