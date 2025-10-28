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

        public CharacterService(ICharacterRepository repo)
        {
            _repo = repo;
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
                Items = items.Select(Map).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CharacterDto?> GetByIdAsync(int id)
        {
            var character = await _repo.GetByIdAsync(id);
            return character == null ? null : Map(character);
        }

        public async Task<CharacterDto> CreateAsync(CreateCharacterDto dto)
        {
            var entity = new Character
            {
                Name = dto.Name,
                Image = dto.Image,
                AgeRange = dto.AgeRange,
                Personality = dto.Personality,
                Description = dto.Description ?? string.Empty,
                AIUrl = dto.AIUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return await GetByIdAsync(entity.CharacterId) ?? throw new Exception("Failed to create Character");
        }

        public async Task<CharacterDto?> UpdatePartialAsync(int id, UpdateCharacterDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            static string? Clean(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                var t = s.Trim();
                if (string.Equals(t, "string", StringComparison.OrdinalIgnoreCase)) return null;
                return t;
            }

            var name = Clean(dto.Name);
            if (name != null) entity.Name = name;

            var image = Clean(dto.Image);
            if (image != null) entity.Image = image;

            if (dto.AgeRange.HasValue) entity.AgeRange = dto.AgeRange.Value;

            var personality = Clean(dto.Personality);
            if (personality != null) entity.Personality = personality;

            var description = Clean(dto.Description);
            if (description != null) entity.Description = description;

            // ✅ THÊM MỚI: Cập nhật AI Model URL
            var aiModelUrl = Clean(dto.AIUrl);
            if (aiModelUrl != null) entity.AIUrl = aiModelUrl;

            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(entity.CharacterId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        private static CharacterDto Map(Character c) => new()
        {
            CharacterId = c.CharacterId,
            Name = c.Name,
            Image = c.Image,
            AgeRange = c.AgeRange,
            Personality = c.Personality,
            Description = c.Description,
            AIUrl = c.AIUrl, 
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };

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
