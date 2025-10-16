using BLL.DTO.UserCharacterDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class UserCharacterService : IUserCharacterService
    {
        private readonly IUserCharacterRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ICharacterRepository _characterRepo;
        private readonly ICharacterPackageRepository _packageRepo;

        public UserCharacterService(
            IUserCharacterRepository repo,
            IUserRepository userRepo,
            ICharacterRepository characterRepo,
            ICharacterPackageRepository packageRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _characterRepo = characterRepo;
            _packageRepo = packageRepo;
        }

        public async Task<List<UserCharacterDto>> GetAllAsync()
        {
            var userCharacters = await _repo.GetAllAsync();
            var dtos = new List<UserCharacterDto>();

            foreach (var uc in userCharacters)
            {
                var user = await _userRepo.GetByIdAsync(uc.UserID);
                var character = await _characterRepo.GetByIdAsync(uc.CharacterID);
                var package = await _packageRepo.GetByIdAsync(uc.PackageId);
                
                dtos.Add(Map(uc, user?.UserName, character?.Name, package?.Name));
            }

            return dtos;
        }

        public async Task<UserCharacterDto?> GetByIdAsync(int id)
        {
            var userCharacter = await _repo.GetByIdAsync(id);
            if (userCharacter == null) return null;

            var user = await _userRepo.GetByIdAsync(userCharacter.UserID);
            var character = await _characterRepo.GetByIdAsync(userCharacter.CharacterID);
            var package = await _packageRepo.GetByIdAsync(userCharacter.PackageId);

            return Map(userCharacter, user?.UserName, character?.Name, package?.Name);
        }

        public async Task<List<UserCharacterDto>> GetByUserIdAsync(int userId)
        {
            var userCharacters = await _repo.GetByUserIdAsync(userId);
            var user = await _userRepo.GetByIdAsync(userId);
            var dtos = new List<UserCharacterDto>();

            foreach (var uc in userCharacters)
            {
                var character = await _characterRepo.GetByIdAsync(uc.CharacterID);
                var package = await _packageRepo.GetByIdAsync(uc.PackageId);
                dtos.Add(Map(uc, user?.UserName, character?.Name, package?.Name));
            }

            return dtos;
        }

        public async Task<List<UserCharacterDto>> GetByCharacterIdAsync(int characterId)
        {
            var userCharacters = await _repo.GetByCharacterIdAsync(characterId);
            var character = await _characterRepo.GetByIdAsync(characterId);
            var dtos = new List<UserCharacterDto>();

            foreach (var uc in userCharacters)
            {
                var user = await _userRepo.GetByIdAsync(uc.UserID);
                var package = await _packageRepo.GetByIdAsync(uc.PackageId);
                dtos.Add(Map(uc, user?.UserName, character?.Name, package?.Name));
            }

            return dtos;
        }

        public async Task<List<UserCharacterDto>> GetByPackageIdAsync(int packageId)
        {
            var userCharacters = await _repo.GetByPackageIdAsync(packageId);
            var package = await _packageRepo.GetByIdAsync(packageId);
            var dtos = new List<UserCharacterDto>();

            foreach (var uc in userCharacters)
            {
                var user = await _userRepo.GetByIdAsync(uc.UserID);
                var character = await _characterRepo.GetByIdAsync(uc.CharacterID);
                dtos.Add(Map(uc, user?.UserName, character?.Name, package?.Name));
            }

            return dtos;
        }

        public async Task<UserCharacterDto?> GetActiveSubscriptionAsync(int userId, int characterId)
        {
            var userCharacter = await _repo.GetActiveSubscriptionAsync(userId, characterId);
            if (userCharacter == null) return null;

            var user = await _userRepo.GetByIdAsync(userCharacter.UserID);
            var character = await _characterRepo.GetByIdAsync(userCharacter.CharacterID);
            var package = await _packageRepo.GetByIdAsync(userCharacter.PackageId);

            return Map(userCharacter, user?.UserName, character?.Name, package?.Name);
        }

        public async Task<UserCharacterDto> CreateAsync(CreateUserCharacterDto dto)
        {
            // Validate User exists
            var user = await _userRepo.GetByIdAsync(dto.UserID);
            if (user == null)
                throw new Exception($"User với ID {dto.UserID} không tồn tại");

            // Validate Character exists
            var character = await _characterRepo.GetByIdAsync(dto.CharacterID);
            if (character == null)
                throw new Exception($"Character với ID {dto.CharacterID} không tồn tại");

            // Validate Package exists
            var package = await _packageRepo.GetByIdAsync(dto.PackageId);
            if (package == null)
                throw new Exception($"Package với ID {dto.PackageId} không tồn tại");

            // Validate StartAt < EndAt
            if (dto.StartAt >= dto.EndAt)
                throw new Exception("StartAt phải nhỏ hơn EndAt");

            var entity = new UserCharacter
            {
                UserID = dto.UserID,
                CharacterID = dto.CharacterID,
                PackageId = dto.PackageId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                AutoRenew = dto.AutoRenew,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return Map(entity, user.UserName, character.Name, package.Name);
        }

        public async Task<UserCharacterDto?> UpdatePartialAsync(int id, UpdateUserCharacterDto dto)
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

            if (dto.StartAt.HasValue)
                entity.StartAt = dto.StartAt.Value;

            if (dto.EndAt.HasValue)
                entity.EndAt = dto.EndAt.Value;

            // Validate StartAt < EndAt after update
            if (entity.StartAt >= entity.EndAt)
                throw new Exception("StartAt phải nhỏ hơn EndAt");

            if (dto.AutoRenew.HasValue)
                entity.AutoRenew = dto.AutoRenew.Value;

            var status = Clean(dto.Status);
            if (status != null) entity.Status = status;

            await _repo.UpdateAsync(entity);

            var user = await _userRepo.GetByIdAsync(entity.UserID);
            var character = await _characterRepo.GetByIdAsync(entity.CharacterID);
            var package = await _packageRepo.GetByIdAsync(entity.PackageId);

            return Map(entity, user?.UserName, character?.Name, package?.Name);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            // Chuyển Status từ "Active" sang "UnActive"
            entity.Status = "UnActive";
            await _repo.UpdateAsync(entity);
            return true;
        }

        private static UserCharacterDto Map(
            UserCharacter uc, 
            string? userName, 
            string? characterName, 
            string? packageName) => new()
        {
            UserCharacterID = uc.UserCharacterID,
            UserID = uc.UserID,
            UserName = userName,
            CharacterID = uc.CharacterID,
            CharacterName = characterName,
            PackageId = uc.PackageId,
            PackageName = packageName,
            StartAt = uc.StartAt,
            EndAt = uc.EndAt,
            AutoRenew = uc.AutoRenew,
            Status = uc.Status,
            CreatedAt = uc.CreatedAt
        };
    }
}