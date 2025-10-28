using BLL.DTO.Common;
using BLL.DTO.DollTypeDTO;
using BLL.Helper;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class DollTypeService : IDollTypeService
    {
        private readonly IDollTypeRepository _repo;
        public DollTypeService(IDollTypeRepository repo) => _repo = repo;

        public async Task<PagedResult<DollTypeDto>> GetAsync(
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize)
        {
            var query = _repo.Query().Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(term) ||
                    (x.Description != null && x.Description.ToLower().Contains(term)));
            }

            var total = await query.CountAsync();

            query = ApplySorting(query, sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = await query.ToListAsync();

            return new PagedResult<DollTypeDto>
            {
                Items = items.Select(Map).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
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

        private static IQueryable<DollType> ApplySorting(
            IQueryable<DollType> query,
            string? sortBy,
            string? sortDir)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(x => x.Create_at);
            }

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLowerInvariant() switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "createdat" => desc ? query.OrderByDescending(x => x.Create_at) : query.OrderBy(x => x.Create_at),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => query.ApplySort(sortBy, sortDir)
            };
        }
    }
}
