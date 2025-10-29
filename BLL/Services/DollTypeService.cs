using AutoMapper;
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
        private readonly IMapper _mapper;
        public DollTypeService(IDollTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

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
                Items = _mapper.Map<List<DollTypeDto>>(items),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DollTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DollTypeDto>(entity);
        }

        public async Task<DollTypeDto> CreateAsync(CreateDollTypeDto dto)
        {
            var entity = _mapper.Map<DollType>(dto);
            await _repo.AddAsync(entity);
            return _mapper.Map<DollTypeDto>(entity);
        }

        public async Task<DollTypeDto?> UpdatePartialAsync(int id, UpdateDollTypeDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            if (entity.Create_at < new DateTime(1753, 1, 1))
                entity.Create_at = DateTime.UtcNow;

            await _repo.UpdateAsync(entity);
            return _mapper.Map<DollTypeDto>(entity);
        }

        public Task<bool> SoftDeleteAsync(int id) => _repo.SoftDeleteAsync(id);
        public Task<bool> HardDeleteAsync(int id) => _repo.HardDeleteAsync(id);

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
