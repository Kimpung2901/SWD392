using BLL.DTO.Common;
using BLL.DTO.DollVariantDTO;
using BLL.Helper;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class DollVariantService : IDollVariantService
    {
        private readonly IDollVariantRepository _repo;

        public DollVariantService(IDollVariantRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<DollVariantDto>> GetAsync(
            int? dollModelId,
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize)
        {
            var query = _repo.Query().Where(v => v.IsActive);

            if (dollModelId.HasValue)
            {
                query = query.Where(v => v.DollModelID == dollModelId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(v =>
                    v.Name.ToLower().Contains(term) ||
                    (v.Color != null && v.Color.ToLower().Contains(term)) ||
                    (v.Size != null && v.Size.ToLower().Contains(term)));
            }

            var total = await query.CountAsync();

            query = ApplySorting(query, sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = await query.ToListAsync();

            return new PagedResult<DollVariantDto>
            {
                Items = items.Select(Map).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DollVariantDto?> GetByIdAsync(int id)
        {
            var variant = await _repo.GetByIdAsync(id);
            return variant == null ? null : Map(variant);
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

        private static IQueryable<DollVariant> ApplySorting(
            IQueryable<DollVariant> query,
            string? sortBy,
            string? sortDir)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(v => v.DollVariantID);
            }

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLowerInvariant() switch
            {
                "name" => desc ? query.OrderByDescending(v => v.Name) : query.OrderBy(v => v.Name),
                "price" => desc ? query.OrderByDescending(v => v.Price) : query.OrderBy(v => v.Price),
                "color" => desc ? query.OrderByDescending(v => v.Color) : query.OrderBy(v => v.Color),
                _ => query.ApplySort(sortBy, sortDir)
            };
        }
    }
}
