using AutoMapper;
using BLL.DTO.Common;
using BLL.DTO.DollModelDTO;
using BLL.Helper;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class DollModelService : IDollModelService
    {
        private readonly IDollModelRepository _repo;
        private readonly IMapper _mapper;

        public DollModelService(IDollModelRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PagedResult<DollModelDto>> GetAsync(
            int? dollTypeId,
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize)
        {
            var query = _repo.Query().Where(m => !m.IsDeleted);

            if (dollTypeId.HasValue)
            {
                query = query.Where(m => m.DollTypeID == dollTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(m =>
                    m.Name.ToLower().Contains(term) ||
                    (m.Description != null && m.Description.ToLower().Contains(term)));
            }

            var total = await query.CountAsync();

            query = ApplySorting(query, sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = await query.ToListAsync();

            return new PagedResult<DollModelDto>
            {
                Items = _mapper.Map<List<DollModelDto>>(items),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DollModelDto?> GetByIdAsync(int id)
        {
            var m = await _repo.GetByIdAsync(id);
            return m == null ? null : _mapper.Map<DollModelDto>(m);
        }

        public async Task<DollModelDto> CreateAsync(CreateDollModelDto dto)
        {
            var entity = _mapper.Map<DollModel>(dto);

            await _repo.AddAsync(entity);
            var saved = await _repo.GetByIdAsync(entity.DollModelID) ?? entity;
            return _mapper.Map<DollModelDto>(saved);
        }

        public async Task<DollModelDto?> UpdatePartialAsync(int id, UpdateDollModelDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            await _repo.UpdateAsync(entity);
            var saved = await _repo.GetByIdAsync(entity.DollModelID) ?? entity;
            return _mapper.Map<DollModelDto>(saved);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            await _repo.SoftDeleteAsync(id);
            return true;
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            await _repo.HardDeleteAsync(id);
            return true;
        }

        private static IQueryable<DollModel> ApplySorting(
            IQueryable<DollModel> query,
            string? sortBy,
            string? sortDir)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(m => m.Create_at);
            }

            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLowerInvariant() switch
            {
                "name" => desc ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
                "createdat" => desc ? query.OrderByDescending(m => m.Create_at) : query.OrderBy(m => m.Create_at),
                "isactive" => desc ? query.OrderByDescending(m => m.IsActive) : query.OrderBy(m => m.IsActive),
                _ => query.ApplySort(sortBy, sortDir)
            };
        }
    }
}
