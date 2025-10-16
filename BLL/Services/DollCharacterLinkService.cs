using BLL.DTO.DollCharacterLinkDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services
{
    public class DollCharacterLinkService : IDollCharacterLinkService
    {
        private readonly IDollCharacterLinkRepository _repo;
        private readonly IOwnedDollRepository _ownedDollRepo;
        private readonly IUserCharacterRepository _userCharacterRepo;

        public DollCharacterLinkService(
            IDollCharacterLinkRepository repo,
            IOwnedDollRepository ownedDollRepo,
            IUserCharacterRepository userCharacterRepo)
        {
            _repo = repo;
            _ownedDollRepo = ownedDollRepo;
            _userCharacterRepo = userCharacterRepo;
        }

        public async Task<List<DollCharacterLinkDto>> GetAllAsync()
        {
            var links = await _repo.GetAllAsync();
            var dtos = new List<DollCharacterLinkDto>();

            foreach (var link in links)
            {
                var ownedDoll = await _ownedDollRepo.GetByIdAsync(link.OwnedDollID);
                var userCharacter = await _userCharacterRepo.GetByIdAsync(link.UserCharacterID);
                dtos.Add(Map(link, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString()));
            }

            return dtos;
        }

        public async Task<DollCharacterLinkDto?> GetByIdAsync(int id)
        {
            var link = await _repo.GetByIdAsync(id);
            if (link == null) return null;

            var ownedDoll = await _ownedDollRepo.GetByIdAsync(link.OwnedDollID);
            var userCharacter = await _userCharacterRepo.GetByIdAsync(link.UserCharacterID);
            return Map(link, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString());
        }

        public async Task<List<DollCharacterLinkDto>> GetByOwnedDollIdAsync(int ownedDollId)
        {
            var links = await _repo.GetByOwnedDollIdAsync(ownedDollId);
            var ownedDoll = await _ownedDollRepo.GetByIdAsync(ownedDollId);
            var dtos = new List<DollCharacterLinkDto>();

            foreach (var link in links)
            {
                var userCharacter = await _userCharacterRepo.GetByIdAsync(link.UserCharacterID);
                dtos.Add(Map(link, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString()));
            }

            return dtos;
        }

        public async Task<List<DollCharacterLinkDto>> GetByUserCharacterIdAsync(int userCharacterId)
        {
            var links = await _repo.GetByUserCharacterIdAsync(userCharacterId);
            var userCharacter = await _userCharacterRepo.GetByIdAsync(userCharacterId);
            var dtos = new List<DollCharacterLinkDto>();

            foreach (var link in links)
            {
                var ownedDoll = await _ownedDollRepo.GetByIdAsync(link.OwnedDollID);
                dtos.Add(Map(link, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString()));
            }

            return dtos;
        }

        public async Task<DollCharacterLinkDto?> GetActiveLinkByOwnedDollIdAsync(int ownedDollId)
        {
            var link = await _repo.GetActiveLinkByOwnedDollIdAsync(ownedDollId);
            if (link == null) return null;

            var ownedDoll = await _ownedDollRepo.GetByIdAsync(link.OwnedDollID);
            var userCharacter = await _userCharacterRepo.GetByIdAsync(link.UserCharacterID);
            return Map(link, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString());
        }

        public async Task<DollCharacterLinkDto> CreateAsync(CreateDollCharacterLinkDto dto)
        {
            // Validate owned doll exists
            var ownedDoll = await _ownedDollRepo.GetByIdAsync(dto.OwnedDollID);
            if (ownedDoll == null)
                throw new Exception($"OwnedDoll với ID {dto.OwnedDollID} không tồn tại");

            // Validate user character exists
            var userCharacter = await _userCharacterRepo.GetByIdAsync(dto.UserCharacterID);
            if (userCharacter == null)
                throw new Exception($"UserCharacter với ID {dto.UserCharacterID} không tồn tại");

            // Check if owned doll already has an active link
            var existingLink = await _repo.GetActiveLinkByOwnedDollIdAsync(dto.OwnedDollID);
            if (existingLink != null)
                throw new Exception($"OwnedDoll #{dto.OwnedDollID} đã được liên kết với Character khác. Vui lòng hủy liên kết cũ trước.");

            var entity = new DollCharacterLink
            {
                OwnedDollID = dto.OwnedDollID,
                UserCharacterID = dto.UserCharacterID,
                BoundAt = dto.BoundAt,
                UnBoundAt = dto.UnBoundAt,
                Note = dto.Note,
                IsActive = true,
                Status = "Active"
            };

            await _repo.AddAsync(entity);
            return Map(entity, ownedDoll.SerialCode, userCharacter.CharacterID.ToString());
        }

        public async Task<DollCharacterLinkDto?> UpdatePartialAsync(int id, UpdateDollCharacterLinkDto dto)
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

            if (dto.BoundAt.HasValue)
                entity.BoundAt = dto.BoundAt.Value;

            if (dto.UnBoundAt.HasValue)
                entity.UnBoundAt = dto.UnBoundAt.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            var note = Clean(dto.Note);
            if (note != null) entity.Note = note;

            var status = Clean(dto.Status);
            if (status != null) entity.Status = status;

            await _repo.UpdateAsync(entity);

            var ownedDoll = await _ownedDollRepo.GetByIdAsync(entity.OwnedDollID);
            var userCharacter = await _userCharacterRepo.GetByIdAsync(entity.UserCharacterID);
            return Map(entity, ownedDoll?.SerialCode, userCharacter?.CharacterID.ToString());
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<bool> HardDeleteAsync(int id)
        {
            return await _repo.HardDeleteAsync(id);
        }

        private static DollCharacterLinkDto Map(DollCharacterLink dcl, string? serialCode, string? characterName) => new()
        {
            LinkID = dcl.LinkID,
            OwnedDollID = dcl.OwnedDollID,
            OwnedDollSerialCode = serialCode,
            UserCharacterID = dcl.UserCharacterID,
            CharacterName = characterName,
            BoundAt = dcl.BoundAt,
            UnBoundAt = dcl.UnBoundAt,
            IsActive = dcl.IsActive,
            Note = dcl.Note,
            Status = dcl.Status
        };
    }
}