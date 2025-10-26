using BLL.DTO.CharacterPackageDTO;
using BLL.IService;
using DAL.Enum;
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

        public async Task<List<CharacterPackageDto>> GetAllAsync()
        {
            var packages = await _repo.GetAllAsync();
            var dtos = new List<CharacterPackageDto>();

            foreach (var pkg in packages)
            {
                var character = await _characterRepo.GetByIdAsync(pkg.CharacterId);
                dtos.Add(Map(pkg, character?.Name));
            }

            return dtos;
        }

        public async Task<CharacterPackageDto?> GetByIdAsync(int id)
        {
            var package = await _repo.GetByIdAsync(id);
            if (package == null) return null;

            var character = await _characterRepo.GetByIdAsync(package.CharacterId);
            return Map(package, character?.Name);
        }

        public async Task<List<CharacterPackageDto>> GetByCharacterIdAsync(int characterId)
        {
            var packages = await _repo.GetByCharacterIdAsync(characterId);
            var character = await _characterRepo.GetByIdAsync(characterId);
            
            return packages.Select(p => Map(p, character?.Name)).ToList();
        }

        public async Task<CharacterPackageDto> CreateAsync(CreateCharacterPackageDto dto)
        {
            // Validate character exists
            var character = await _characterRepo.GetByIdAsync(dto.CharacterId);
            if (character == null)
                throw new Exception($"Character với ID {dto.CharacterId} không tồn tại");

            var entity = new CharacterPackage
            {
                CharacterId = dto.CharacterId,
                Name = dto.Name,
                DurationDays = dto.DurationDays,
                Billing_Cycle = dto.Billing_Cycle,
                Price = dto.Price,
                Description = dto.Description,
                IsActive = true,
                Status = CharacterPackageStatus.Active, // <-- FIXED HERE
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return Map(entity, character.Name);
        }

        public async Task<CharacterPackageDto?> UpdatePartialAsync(int id, UpdateCharacterPackageDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            // Clean and update helper function
            static string? Clean(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                var t = s.Trim();
                if (string.Equals(t, "string", StringComparison.OrdinalIgnoreCase)) return null;
                return t;
            }

            var name = Clean(dto.Name);
            if (name != null) entity.Name = name;

            if (dto.DurationDays.HasValue && dto.DurationDays.Value > 0)
                entity.DurationDays = dto.DurationDays.Value;

            var billingCycle = Clean(dto.Billing_Cycle);
            if (billingCycle != null) entity.Billing_Cycle = billingCycle;

            if (dto.Price.HasValue && dto.Price.Value > 0)
                entity.Price = dto.Price.Value;

            var description = Clean(dto.Description);
            if (description != null) entity.Description = description;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            // FIX: Convert string status to enum
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (Enum.TryParse<CharacterPackageStatus>(dto.Status, true, out var parsedStatus))
                {
                    entity.Status = parsedStatus;
                }
            }

            await _repo.UpdateAsync(entity);

            var character = await _characterRepo.GetByIdAsync(entity.CharacterId);
            return Map(entity, character?.Name);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            return await _repo.HardDeleteAsync(id);
        }

        private static CharacterPackageDto Map(CharacterPackage p, string? characterName) => new()
        {
            PackageId = p.PackageId,
            CharacterId = p.CharacterId,
            CharacterName = characterName,
            Name = p.Name,
            DurationDays = (int)p.DurationDays,
            Billing_Cycle = p.Billing_Cycle,
            Price = p.Price,
            Description = p.Description,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            Status = p.Status
        };
    }
}