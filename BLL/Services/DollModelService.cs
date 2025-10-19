using BLL.DTO.DollModelDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class DollModelService : IDollModelService
    {
        private readonly IDollModelRepository _repo;

        public DollModelService(IDollModelRepository repo)
        {
            _repo = repo;
        }

        // GetAllAsync
        public async Task<List<DollModelDto>> GetAllAsync()
        {
            var models = await _repo.GetAllAsync(); 
            return models.Select(m => new DollModelDto
            {
                DollModelID = m.DollModelID,
                DollTypeID = m.DollTypeID,
                DollTypeName = m.DollType?.Name, 
                Name = m.Name,
                Description = m.Description,
                Create_at = m.Create_at,
                Image = m.Image,
                IsActive = m.IsActive
            }).ToList();
        }

        // GetByIdAsync
        public async Task<DollModelDto?> GetByIdAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            if (m == null) return null;

            return new DollModelDto
            {
                DollModelID = m.DollModelID,
                DollTypeID = m.DollTypeID,
                DollTypeName = m.DollType?.Name, 
                Name = m.Name,
                Description = m.Description,
                Create_at = m.Create_at,
                Image = m.Image,
                IsActive = m.IsActive
            };
        }

        // GetByDollTypeIdAsync - NEW METHOD
        public async Task<List<DollModelDto>> GetByDollTypeIdAsync(int dollTypeId)
        {
            var models = await _repo.GetByTypeIdAsync(dollTypeId);
            return models.Select(m => new DollModelDto
            {
                DollModelID = m.DollModelID,
                DollTypeID = m.DollTypeID,
                DollTypeName = m.DollType?.Name,
                Name = m.Name,
                Description = m.Description,
                Create_at = m.Create_at,
                Image = m.Image,
                IsActive = m.IsActive
            }).ToList();
        }

        public async Task<DollModelDto> CreateAsync(CreateDollModelDto dto)
        {
            var entity = new DollModel
            {
                DollTypeID = dto.DollTypeID,
                Name = dto.Name,
                Description = dto.Description,
                Image = dto.Image,
                Create_at = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };
            
            await _repo.AddAsync(entity);
            return await GetByIdAsync(entity.DollModelID) ?? throw new Exception("Failed to create DollModel");
        }

        public async Task<DollModelDto?> UpdatePartialAsync(int id, UpdateDollModelDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.Image != null) entity.Image = dto.Image;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(entity.DollModelID);
        }

        public Task<bool> SoftDeleteAsync(int id)
            => _repo.SoftDeleteAsync(id).ContinueWith(_ => true);

        public Task<bool> HardDeleteAsync(int id)
            => _repo.HardDeleteAsync(id).ContinueWith(_ => true);
    }
}
