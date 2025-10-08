using BLL.DTO.DollTypeDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class DollTypeService : IDollTypeService
    {
        private readonly IDollTypeRepository _repo;
        public DollTypeService(IDollTypeRepository repo) => _repo = repo;

        public async Task<List<DollTypeDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(Map).ToList();
        }

        public async Task<DollTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto)
        {
            var entity = new DollType
            {
                Name = dto.Name,
                Description = dto.Description,
                Image = dto.Image,
                IsActive = true,
                IsDeleted = false,
                Create_at = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);
            return Map(entity);
        }

      
        public async Task<DollTypeDto?> UpdatePartialAsync(int id, UpdateDollTypeDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

           
            if (!string.IsNullOrWhiteSpace(dto.Name)) 
                entity.Name = dto.Name.Trim();
            
            if (!string.IsNullOrWhiteSpace(dto.Description)) 
                entity.Description = dto.Description.Trim();
            
            if (!string.IsNullOrWhiteSpace(dto.Image)) 
                entity.Image = dto.Image.Trim();
     
            if (dto.IsActive.HasValue) 
                entity.IsActive = dto.IsActive.Value;

            if (entity.Create_at < new DateTime(1753, 1, 1))
                entity.Create_at = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            return Map(entity);
        }

        public Task<bool> SoftDeleteAsync(int id) => _repo.SoftDeleteAsync(id);
        public Task<bool> HardDeleteAsync(int id) => _repo.HardDeleteAsync(id);

        private static DollTypeDto Map(DollType e) => new()
        {
            DollTypeID = e.DollTypeID,
            Name = e.Name,
            Description = e.Description,
            Image = e.Image,
            Create_at = e.Create_at,
            IsActive = e.IsActive,
            IsDeleted = e.IsDeleted
        };
    }
}
