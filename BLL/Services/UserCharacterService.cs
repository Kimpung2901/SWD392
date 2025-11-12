using AutoMapper;
using AutoMapper.QueryableExtensions;
using BLL.DTO.UserCharacterDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;
using DAL.Enum;
using Microsoft.EntityFrameworkCore;
using BLL.Helper;

namespace BLL.Services
{
    public class UserCharacterService : IUserCharacterService
    {
        private readonly IUserCharacterRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ICharacterRepository _characterRepo;
        private readonly ICharacterPackageRepository _packageRepo;
        private readonly DollDbContext _db;
        private readonly IMapper _mapper;

        public UserCharacterService(
            IUserCharacterRepository repo,
            IUserRepository userRepo,
            ICharacterRepository characterRepo,
            ICharacterPackageRepository packageRepo,
            DollDbContext db,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _characterRepo = characterRepo;
            _packageRepo = packageRepo;
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<UserCharacterDto>> GetAllAsync()
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<UserCharacterDto?> GetByIdAsync(int id)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)
                .Where(uc => uc.UserCharacterID == id)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserCharacterDto>> GetByUserIdAsync(int userId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)
                .Where(uc => uc.UserID == userId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<UserCharacterDto>> GetByCharacterIdAsync(int characterId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Package)
                .Where(uc => uc.CharacterID == characterId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<UserCharacterDto>> GetByPackageIdAsync(int packageId)
        {
            return await _db.UserCharacters
                .Include(uc => uc.User)
                .Include(uc => uc.Character)
                .Where(uc => uc.PackageId == packageId)
                .OrderByDescending(uc => uc.CreatedAt)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<UserCharacterDto>> GetActiveSubscriptionsAsync(int userId)
        {
            var vietnamNow = DateTimeHelper.GetVietnamTime();
            return await _db.UserCharacters
                .Include(uc => uc.Character)
                .Include(uc => uc.Package)
                .Where(uc => uc.UserID == userId 
                    && uc.Status == UserCharacterStatus.Active
                    && uc.EndAt > vietnamNow)
                .OrderByDescending(uc => uc.EndAt)
                .AsNoTracking()
                .ProjectTo<UserCharacterDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<UserCharacterDto> CreateAsync(CreateUserCharacterDto dto)
        {
            // Validate user exists
            var user = await _userRepo.GetByIdAsync(dto.UserID);
            if (user == null)
                throw new InvalidOperationException($"User với ID {dto.UserID} không tồn tại");

            // Validate character exists
            var character = await _characterRepo.GetByIdAsync(dto.CharacterID);
            if (character == null)
                throw new InvalidOperationException($"Character với ID {dto.CharacterID} không tồn tại");

            // Validate package exists and belongs to character
            var package = await _packageRepo.GetByIdAsync(dto.PackageId);
            if (package == null)
                throw new InvalidOperationException($"Package với ID {dto.PackageId} không tồn tại");

            if (package.CharacterId != dto.CharacterID)
                throw new InvalidOperationException("Package không thuộc về Character đã chọn");

            var vietnamNow = DateTimeHelper.GetVietnamTime();
            var entity = new UserCharacter
            {
                UserID = dto.UserID,
                CharacterID = dto.CharacterID,
                PackageId = dto.PackageId,
                StartAt = dto.StartAt ?? vietnamNow,
                EndAt = dto.EndAt ?? vietnamNow.AddDays(package.DurationDays),
                AutoRenew = dto.AutoRenew,
                Status = dto.Status,
                CreatedAt = vietnamNow
            };

            await _repo.AddAsync(entity);
            return await GetByIdAsync(entity.UserCharacterID) 
                ?? throw new Exception("Không thể tạo UserCharacter");
        }

        public async Task<UserCharacterDto?> UpdatePartialAsync(int id, UpdateUserCharacterDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.StartAt.HasValue)
                entity.StartAt = dto.StartAt.Value;

            if (dto.EndAt.HasValue)
                entity.EndAt = dto.EndAt.Value;

            if (dto.AutoRenew.HasValue)
                entity.AutoRenew = dto.AutoRenew.Value;

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            await _repo.UpdateAsync(entity);
            return await GetByIdAsync(entity.UserCharacterID);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<bool> RenewSubscriptionAsync(int userCharacterId)
        {
            var entity = await _repo.GetByIdAsync(userCharacterId);
            if (entity == null) return false;

            var package = await _packageRepo.GetByIdAsync(entity.PackageId);
            if (package == null) 
                throw new InvalidOperationException("Package không tồn tại");

            // Extend subscription
            var vietnamNow = DateTimeHelper.GetVietnamTime();
            if (entity.EndAt < vietnamNow)
            {
                // Expired - start from now
                entity.StartAt = vietnamNow;
                entity.EndAt = vietnamNow.AddDays(package.DurationDays);
            }
            else
            {
                // Active - extend from current end date
                entity.EndAt = entity.EndAt.AddDays(package.DurationDays);
            }

            entity.Status = UserCharacterStatus.Active;
            await _repo.UpdateAsync(entity);
            return true;
        }
    }
}