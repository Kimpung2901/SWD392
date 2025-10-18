using BLL.DTO.DollCharacterLinkDTO;
using BLL.IService;
using DAL.IRepo;
using DAL.Models;

namespace BLL.Services;

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
            dtos.Add(await MapToDtoAsync(link));
        }

        return dtos;
    }

    public async Task<DollCharacterLinkDto?> GetByIdAsync(int id)
    {
        var link = await _repo.GetByIdAsync(id);
        return link == null ? null : await MapToDtoAsync(link);
    }

    public async Task<List<DollCharacterLinkDto>> GetByOwnedDollIdAsync(int ownedDollId)
    {
        var links = await _repo.GetByOwnedDollIdAsync(ownedDollId);
        var dtos = new List<DollCharacterLinkDto>();

        foreach (var link in links)
        {
            dtos.Add(await MapToDtoAsync(link));
        }

        return dtos;
    }

    public async Task<List<DollCharacterLinkDto>> GetByUserCharacterIdAsync(int userCharacterId)
    {
        var links = await _repo.GetByUserCharacterIdAsync(userCharacterId);
        var dtos = new List<DollCharacterLinkDto>();

        foreach (var link in links)
        {
            dtos.Add(await MapToDtoAsync(link));
        }

        return dtos;
    }

    public async Task<List<DollCharacterLinkDto>> GetActiveLinksAsync()
    {
        var links = await _repo.GetActiveLinksAsync();
        var dtos = new List<DollCharacterLinkDto>();

        foreach (var link in links)
        {
            dtos.Add(await MapToDtoAsync(link));
        }

        return dtos;
    }

    public async Task<DollCharacterLinkDto> CreateAsync(CreateDollCharacterLinkDto dto)
    {
        // Validate OwnedDoll exists
        var ownedDoll = await _ownedDollRepo.GetByIdAsync(dto.OwnedDollID);
        if (ownedDoll == null)
            throw new Exception($"OwnedDoll với ID {dto.OwnedDollID} không tồn tại");

        // Validate UserCharacter exists
        var userCharacter = await _userCharacterRepo.GetByIdAsync(dto.UserCharacterID);
        if (userCharacter == null)
            throw new Exception($"UserCharacter với ID {dto.UserCharacterID} không tồn tại");

        // Check if OwnedDoll already has an active link
        var existingLink = await _repo.GetActiveLinkByOwnedDollIdAsync(dto.OwnedDollID);
        if (existingLink != null)
            throw new Exception($"OwnedDoll #{dto.OwnedDollID} đã được bind với character khác. Vui lòng unbind trước.");

        // Create new link
        var entity = new DollCharacterLink
        {
            OwnedDollID = dto.OwnedDollID,
            UserCharacterID = dto.UserCharacterID,
            BoundAt = dto.BoundAt ?? DateTime.UtcNow,
            UnBoundAt = DateTime.MinValue, // Not unbound yet
            IsActive = true,
            Status = "Active",
            Note = dto.Note ?? string.Empty
        };

        await _repo.AddAsync(entity);

        return await GetByIdAsync(entity.LinkID) ?? throw new Exception("Failed to create link");
    }

    public async Task<DollCharacterLinkDto?> UpdatePartialAsync(int id, UpdateDollCharacterLinkDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Note))
            entity.Note = dto.Note.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Status))
            entity.Status = dto.Status.Trim();

        if (dto.IsActive.HasValue)
            entity.IsActive = dto.IsActive.Value;

        await _repo.UpdateAsync(entity);

        return await GetByIdAsync(id);
    }

    public async Task<bool> UnbindAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;

        if (!entity.IsActive)
            throw new Exception("Link này đã được unbind rồi");

        entity.IsActive = false;
        entity.Status = "Unbound";
        entity.UnBoundAt = DateTime.UtcNow;

        await _repo.UpdateAsync(entity);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _repo.DeleteAsync(id);
        return true;
    }

    private async Task<DollCharacterLinkDto> MapToDtoAsync(DollCharacterLink link)
    {
        var ownedDoll = await _ownedDollRepo.GetByIdAsync(link.OwnedDollID);
        var userCharacter = await _userCharacterRepo.GetByIdAsync(link.UserCharacterID);

        return new DollCharacterLinkDto
        {
            LinkID = link.LinkID,
            OwnedDollID = link.OwnedDollID,
            OwnedDollSerialCode = ownedDoll?.SerialCode,
            UserCharacterID = link.UserCharacterID,
            CharacterName = userCharacter?.Character?.Name,
            BoundAt = link.BoundAt,
            UnBoundAt = link.UnBoundAt,
            IsActive = link.IsActive,
            Note = link.Note,
            Status = link.Status
        };
    }
}