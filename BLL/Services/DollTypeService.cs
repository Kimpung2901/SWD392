// BLL/Services/DollTypeService.cs
using DAL.Models;
using BLL.DTO.DollTypeDTO;
using DAL.Repositories;

namespace BLL.Services
{
    public class DollTypeService : IDollTypeService
    {
        private readonly DollTypeRepository _repo;
        public DollTypeService(DollTypeRepository repo) => _repo = repo;

        private static DollTypeDto Map(DollType x) => new()
        {
            DollTypeID = x.DollTypeID,
            Name = x.Name,
            Description = x.Description,
            Create_at = x.Create_at,
            Image = x.Image,
            IsActive = x.IsActive,
            IsDeleted = x.IsDeleted
        };

        public async Task<List<DollTypeDto>> GetAllAsync(bool includeDeleted = false) =>
            (await _repo.GetAllAsync(includeDeleted)).Select(Map).ToList();

        public async Task<DollTypeDto?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            var x = await _repo.GetByIdAsync(id, includeDeleted);
            return x is null ? null : Map(x);
        }

        public async Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto)
        {
            var entity = new DollType
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = dto.Image,
                Create_at = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };
            await _repo.AddAsync(entity);
            return Map(entity);
        }

        public async Task<DollTypeDto?> UpdateAsync(int id, string name, string description, string image, bool isActive)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Name = name;
            entity.Description = description;
            entity.Image = image;
            entity.IsActive = isActive;

            await _repo.UpdateAsync(entity);
            return Map(entity);
        }

        // Soft delete
        public async Task<DollTypeDto?> DeleteSoftAsync(int id)
        {
            var entity = await _repo.SoftDeleteAsync(id);
            return entity is null ? null : Map(entity);
        }

        // Restore
        public async Task<DollTypeDto?> RestoreAsync(int id)
        {
            var entity = await _repo.RestoreAsync(id);
            return entity is null ? null : Map(entity);
        }

        // Hard delete
        public async Task<DollTypeDto?> DeleteHardAsync(int id)
        {
            var entity = await _repo.HardDeleteAsync(id);
            return entity is null ? null : Map(entity);
        }
    }
}
