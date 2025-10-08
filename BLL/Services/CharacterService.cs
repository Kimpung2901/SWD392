using BLL.DTO.CharacterDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepository _repo;

        public CharacterService(ICharacterRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CharacterDto>> GetAllAsync()
        {
            var characters = await _repo.GetAllAsync();
            return characters.Select(Map).ToList();
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
                Gender = dto.Gender,
                Language = dto.Language,
                AgeRange = dto.AgeRange,
                Personality = dto.Personality,
                Description = dto.Description,
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

            var gender = Clean(dto.Gender);
            if (gender != null) entity.Gender = gender;

            var language = Clean(dto.Language);
            if (language != null) entity.Language = language;

            if (dto.AgeRange.HasValue) entity.AgeRange = dto.AgeRange.Value;

            var personality = Clean(dto.Personality);
            if (personality != null) entity.Personality = personality;

            var description = Clean(dto.Description);
            if (description != null) entity.Description = description;

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
            Gender = c.Gender,
            Language = c.Language,
            AgeRange = c.AgeRange,
            Personality = c.Personality,
            Description = c.Description,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt
        };
    }
}