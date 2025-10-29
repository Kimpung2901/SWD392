using AutoMapper;
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
        private readonly IMapper _mapper;

        public DollVariantService(IDollVariantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
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
                Items = _mapper.Map<List<DollVariantDto>>(items),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<DollVariantDto>> GetByDollModelIdAsync(int dollModelId)
        {
            var variants = await _repo.GetByDollModelIdAsync(dollModelId);
            return _mapper.Map<List<DollVariantDto>>(variants);
        }

        public async Task<DollVariantDto?> GetByIdAsync(int id)
        {
            var variant = await _repo.GetByIdAsync(id);
            return variant == null ? null : _mapper.Map<DollVariantDto>(variant);
        }

        public async Task<DollVariantDto> CreateAsync(CreateDollVariantDto dto)
        {
            var entity = _mapper.Map<DollVariant>(dto);

            await _repo.AddAsync(entity);

            var saved = await _repo.GetByIdAsync(entity.DollVariantID) ?? entity;
            return _mapper.Map<DollVariantDto>(saved);
        }

        public async Task<DollVariantDto?> UpdatePartialAsync(int id, UpdateDollVariantDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            await _repo.UpdateAsync(entity);

            var saved = await _repo.GetByIdAsync(id) ?? entity;
            return _mapper.Map<DollVariantDto>(saved);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

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
