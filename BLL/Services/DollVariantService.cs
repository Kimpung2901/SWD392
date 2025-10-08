using BLL.DTO.DollVariantDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class DollVariantService : IDollVariantService
    {
        private readonly IDollVariantRepository _repo;

        public DollVariantService(IDollVariantRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<DollVariantDto>> GetAllAsync()
        {
            var variants = await _repo.GetAllAsync();
            return variants.Select(Map).ToList();
        }

        public async Task<DollVariantDto?> GetByIdAsync(int id)
        {
            var variant = await _repo.GetByIdAsync(id);
            return variant == null ? null : Map(variant);
        }

        public async Task<List<DollVariantDto>> GetByDollModelIdAsync(int dollModelId)
        {
            var variants = await _repo.GetByDollModelIdAsync(dollModelId);
            return variants.Select(Map).ToList();
        }

        public async Task<DollVariantDto> CreateAsync(CreateDollVariantDto dto)
        {
            var entity = new DollVariant
            {
                DollModelID = dto.DollModelID,
                Name = dto.Name,
                Price = dto.Price,
                Color = dto.Color,
                Size = dto.Size,
                Image = dto.Image,
                IsActive = true
            };

            await _repo.AddAsync(entity);
            
            return Map(await _repo.GetByIdAsync(entity.DollVariantID) ?? entity);
        }

        public async Task<DollVariantDto?> UpdatePartialAsync(int id, UpdateDollVariantDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Name)) entity.Name = dto.Name;
            if (dto.Price.HasValue && dto.Price.Value > 0) entity.Price = dto.Price.Value;
            if (!string.IsNullOrWhiteSpace(dto.Color)) entity.Color = dto.Color;
            if (!string.IsNullOrWhiteSpace(dto.Size)) entity.Size = dto.Size;
            if (!string.IsNullOrWhiteSpace(dto.Image)) entity.Image = dto.Image;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _repo.UpdateAsync(entity);
            
            return Map(await _repo.GetByIdAsync(id) ?? entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        private static DollVariantDto Map(DollVariant v) => new()
        {
            DollVariantID = v.DollVariantID,
            DollModelID = v.DollModelID,
            DollModelName = v.DollModel?.Name ?? string.Empty,
            Name = v.Name,
            Price = v.Price,
            Color = v.Color,
            Size = v.Size,
            Image = v.Image,
            IsActive = v.IsActive
        };
    }
}