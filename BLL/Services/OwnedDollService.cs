using BLL.DTO.OwnedDollDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class OwnedDollService : IOwnedDollService
    {
        private readonly IOwnedDollRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IDollVariantRepository _dollVariantRepo;

        public OwnedDollService(
            IOwnedDollRepository repo,
            IUserRepository userRepo,
            IDollVariantRepository dollVariantRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _dollVariantRepo = dollVariantRepo;
        }

        public async Task<List<OwnedDollDto>> GetAllAsync()
        {
            var ownedDolls = await _repo.GetAllAsync();
            var dtos = new List<OwnedDollDto>();

            foreach (var od in ownedDolls)
            {
                var user = await _userRepo.GetByIdAsync(od.UserID);
                var dollVariant = await _dollVariantRepo.GetByIdAsync(od.DollVariantID);
                dtos.Add(Map(od, user?.UserName, dollVariant?.Name));
            }

            return dtos;
        }

        public async Task<OwnedDollDto?> GetByIdAsync(int id)
        {
            var ownedDoll = await _repo.GetByIdAsync(id);
            if (ownedDoll == null) return null;

            var user = await _userRepo.GetByIdAsync(ownedDoll.UserID);
            var dollVariant = await _dollVariantRepo.GetByIdAsync(ownedDoll.DollVariantID);
            return Map(ownedDoll, user?.UserName, dollVariant?.Name);
        }

        public async Task<List<OwnedDollDto>> GetByUserIdAsync(int userId)
        {
            var ownedDolls = await _repo.GetByUserIdAsync(userId);
            var user = await _userRepo.GetByIdAsync(userId);
            var dtos = new List<OwnedDollDto>();

            foreach (var od in ownedDolls)
            {
                var dollVariant = await _dollVariantRepo.GetByIdAsync(od.DollVariantID);
                dtos.Add(Map(od, user?.UserName, dollVariant?.Name));
            }

            return dtos;
        }

        public async Task<List<OwnedDollDto>> GetByDollVariantIdAsync(int dollVariantId)
        {
            var ownedDolls = await _repo.GetByDollVariantIdAsync(dollVariantId);
            var dollVariant = await _dollVariantRepo.GetByIdAsync(dollVariantId);
            var dtos = new List<OwnedDollDto>();

            foreach (var od in ownedDolls)
            {
                var user = await _userRepo.GetByIdAsync(od.UserID);
                dtos.Add(Map(od, user?.UserName, dollVariant?.Name));
            }

            return dtos;
        }

        public async Task<OwnedDollDto> CreateAsync(CreateOwnedDollDto dto)
        {
            // Validate user exists
            var user = await _userRepo.GetByIdAsync(dto.UserID);
            if (user == null)
                throw new Exception($"User với ID {dto.UserID} không tồn tại");

            // Validate doll variant exists
            var dollVariant = await _dollVariantRepo.GetByIdAsync(dto.DollVariantID);
            if (dollVariant == null)
                throw new Exception($"DollVariant với ID {dto.DollVariantID} không tồn tại");

            var entity = new OwnedDoll
            {
                UserID = dto.UserID,
                DollVariantID = dto.DollVariantID,
                SerialCode = dto.SerialCode,
                Status = "Active",
                Acquired_at = dto.Acquired_at,
                Expired_at = dto.Expired_at
            };

            await _repo.AddAsync(entity);
            return Map(entity, user.UserName, dollVariant.Name);
        }

        public async Task<OwnedDollDto?> UpdatePartialAsync(int id, UpdateOwnedDollDto dto)
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

            var serialCode = Clean(dto.SerialCode);
            if (serialCode != null) entity.SerialCode = serialCode;

            var status = Clean(dto.Status);
            if (status != null) entity.Status = status;

            if (dto.Acquired_at.HasValue)
                entity.Acquired_at = dto.Acquired_at.Value;

            if (dto.Expired_at.HasValue)
                entity.Expired_at = dto.Expired_at.Value;

            await _repo.UpdateAsync(entity);

            var user = await _userRepo.GetByIdAsync(entity.UserID);
            var dollVariant = await _dollVariantRepo.GetByIdAsync(entity.DollVariantID);
            return Map(entity, user?.UserName, dollVariant?.Name);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            return await _repo.HardDeleteAsync(id);
        }

        private static OwnedDollDto Map(OwnedDoll od, string? userName, string? dollVariantName) => new()
        {
            OwnedDollID = od.OwnedDollID,
            UserID = od.UserID,
            UserName = userName,
            DollVariantID = od.DollVariantID,
            DollVariantName = dollVariantName,
            SerialCode = od.SerialCode,
            Status = od.Status,
            Acquired_at = od.Acquired_at,
            Expired_at = od.Expired_at
        };
    }
}