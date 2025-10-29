using AutoMapper;
using BLL.DTO.CharacterDTO;
using BLL.DTO.Common;
using BLL.Helper;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _repo;
        private readonly IMapper _mapper;

        public CharacterService(ICharacterRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PagedResult<CharacterDto>> GetAsync(
            string? search,
            string? sortBy,
            string? sortDir,
            int page,
            int pageSize)
        {
            var query = _repo.Query().Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLowerInvariant();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    (c.Personality != null && c.Personality.ToLower().Contains(term)) ||
                    (c.Description != null && c.Description.ToLower().Contains(term)));
            }

            var total = await query.CountAsync();

            query = ApplySorting(query, sortBy, sortDir);
            query = query.ApplyPagination(page, pageSize);

            var items = await query.ToListAsync();

            return new PagedResult<CharacterDto>
            {
                Items = _mapper.Map<List<CharacterDto>>(items),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CharacterDto?> GetByIdAsync(int id)
        {
            var character = await _repo.GetByIdAsync(id);
            return character == null ? null : _mapper.Map<CharacterDto>(character);
        }

        public async Task<CharacterDto> CreateAsync(CreateCharacterDto dto)
        {
            var entity = _mapper.Map<Character>(dto);
            entity.Name = entity.Name.Trim();
            entity.Image = entity.Image.Trim();
            entity.Personality = entity.Personality?.Trim() ?? string.Empty;
            entity.Description = entity.Description?.Trim() ?? string.Empty;
            entity.AIUrl = Clean(entity.AIUrl) ?? entity.AIUrl;

            await _repo.AddAsync(entity);
            var saved = await _repo.GetByIdAsync(entity.CharacterId) ?? entity;
            return _mapper.Map<CharacterDto>(saved);
        }

        public async Task<CharacterDto?> UpdatePartialAsync(int id, UpdateCharacterDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var originalName = entity.Name;
            var originalImage = entity.Image;
            var originalPersonality = entity.Personality;
            var originalDescription = entity.Description;
            var originalAiUrl = entity.AIUrl;

            _mapper.Map(dto, entity);

            var cleanedName = Clean(entity.Name);
            entity.Name = cleanedName ?? originalName;

            var cleanedImage = Clean(entity.Image);
            entity.Image = cleanedImage ?? originalImage;

            if (dto.AgeRange.HasValue) entity.AgeRange = dto.AgeRange.Value;

            var cleanedPersonality = Clean(entity.Personality);
            entity.Personality = cleanedPersonality ?? originalPersonality;

            var cleanedDescription = Clean(entity.Description);
            entity.Description = cleanedDescription ?? originalDescription;

            var cleanedAiUrl = Clean(entity.AIUrl);
            entity.AIUrl = cleanedAiUrl ?? originalAiUrl;

            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _repo.UpdateAsync(entity);
            var saved = await _repo.GetByIdAsync(entity.CharacterId) ?? entity;
            return _mapper.Map<CharacterDto>(saved);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        private static string? Clean(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            var trimmed = value.Trim();
            return string.Equals(trimmed, "string", StringComparison.OrdinalIgnoreCase) ? null : trimmed;
        }

        private static IQueryable<Character> ApplySorting(
            IQueryable<Character> query,
            string? sortBy,
            string? sortDir)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(c => c.CreatedAt);
            }

            var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLowerInvariant() switch
            {
                "name" => isDesc
                    ? query.OrderByDescending(c => c.Name)
                    : query.OrderBy(c => c.Name),
                "agerange" => isDesc
                    ? query.OrderByDescending(c => c.AgeRange)
                    : query.OrderBy(c => c.AgeRange),
                "createdat" => isDesc
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CreatedAt),
                _ => query.ApplySort(sortBy, sortDir)
            };
        }
    }
}




