using BLL.DTO.CharacterPackageDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class CharacterPackageService : ICharacterPackageService
    {
        private readonly ICharacterPackageRepository _repo;
        private readonly ICharacterRepository _characterRepo;

        public CharacterPackageService(
            ICharacterPackageRepository repo,
            ICharacterRepository characterRepo)
        {
            _repo = repo;
            _characterRepo = characterRepo;
        }

        public async Task<List<CharacterPackage>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<CharacterPackage?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<CharacterPackage>> GetByCharacterIdAsync(int characterId)
        {
            return await _repo.GetByCharacterIdAsync(characterId);
        }

        public async Task<CharacterPackage> CreateAsync(CreateCharacterPackageDto dto)
        {
            // Validate CharacterId exists
            var character = await _characterRepo.GetByIdAsync(dto.CharacterId);
            if (character == null)
                throw new ArgumentException($"Character with ID {dto.CharacterId} does not exist");

            var entity = new CharacterPackage
            {
                CharacterId = dto.CharacterId,
                Name = dto.Name,
                Billing_Cycle = dto.Billing_Cycle,
                Price = dto.Price,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Status = dto.Status
            };

            await _repo.AddAsync(entity);
            return entity;
        }

        public async Task<CharacterPackage?> UpdatePartialAsync(int id, UpdateCharacterPackageDto dto)
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

            // Validate CharacterId if changed
            if (dto.CharacterId.HasValue)
            {
                var character = await _characterRepo.GetByIdAsync(dto.CharacterId.Value);
                if (character == null)
                    throw new ArgumentException($"Character with ID {dto.CharacterId.Value} does not exist");
                entity.CharacterId = dto.CharacterId.Value;
            }

            var name = Clean(dto.Name);
            if (name != null) entity.Name = name;

            var billingCycle = Clean(dto.Billing_Cycle);
            if (billingCycle != null) entity.Billing_Cycle = billingCycle;

            if (dto.Price.HasValue) entity.Price = dto.Price.Value;

            var description = Clean(dto.Description);
            if (description != null) entity.Description = description;

            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            var status = Clean(dto.Status);
            if (status != null) entity.Status = status;

            await _repo.UpdateAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}