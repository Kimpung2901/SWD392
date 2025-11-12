using BLL.IService;
using DAL.Enum;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class UserCharacterManager : IUserCharacterManager
{
    private readonly DollDbContext _db;
    private readonly ILogger<UserCharacterManager> _logger;

    public UserCharacterManager(DollDbContext db, ILogger<UserCharacterManager> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> EnsureUserCharacterForOrderAsync(CharacterOrder characterOrder, string? contextTag = null)
    {
        var tag = string.IsNullOrWhiteSpace(contextTag) ? nameof(UserCharacterManager) : contextTag;

        // ✅ FIX 1: Validate required fields - Kiểm tra UserID > 0 (vì là int, không nullable)
        if (characterOrder.UserID <= 0 || characterOrder.CharacterID <= 0 || characterOrder.PackageID <= 0)
        {
            _logger.LogWarning(
                "[{Tag}] CharacterOrder #{OrderId} missing required fields (UserID={UserId}, CharacterID={CharId}, PackageID={PkgId})",
                tag,
                characterOrder.CharacterOrderID,
                characterOrder.UserID,
                characterOrder.CharacterID,
                characterOrder.PackageID);
            return false;
        }

        // Load package to get duration
        var package = characterOrder.Package ?? await _db.CharacterPackages
            .FirstOrDefaultAsync(p => p.PackageId == characterOrder.PackageID);

        if (package == null)
        {
            _logger.LogWarning(
                "[{Tag}] Package #{PackageId} not found for CharacterOrder #{OrderId}",
                tag,
                characterOrder.PackageID,
                characterOrder.CharacterOrderID);
            return false;
        }

        // Check if UserCharacter already exists (active and not expired)
        var existingUserChar = await _db.UserCharacters
            .FirstOrDefaultAsync(uc =>
                uc.UserID == characterOrder.UserID &&
                uc.CharacterID == characterOrder.CharacterID &&
                uc.PackageId == characterOrder.PackageID &&
                uc.Status == UserCharacterStatus.Active &&
                uc.EndAt > DateTime.UtcNow);

        if (existingUserChar != null)
        {
            _logger.LogInformation(
                "[{Tag}] UserCharacter already exists for User #{UserId}, Character #{CharId}, Package #{PkgId}",
                tag,
                characterOrder.UserID,
                characterOrder.CharacterID,
                characterOrder.PackageID);
            return false;
        }

        // Create new UserCharacter
        var now = DateTime.UtcNow;
        var userCharacter = new UserCharacter
        {
            UserID = characterOrder.UserID,
            CharacterID = characterOrder.CharacterID,
            PackageId = characterOrder.PackageID,
            StartAt = now,
            EndAt = now.AddDays(package.DurationDays),
            AutoRenew = false,
            Status = UserCharacterStatus.Active,
            CreatedAt = now
        };

        // ✅ FIX 2: Add là synchronous, KHÔNG cần await
        // Nhưng CẦN return true để caller biết cần SaveChanges
        _db.UserCharacters.Add(userCharacter);

        _logger.LogInformation(
            "[{Tag}] Created UserCharacter for User #{UserId}, Character #{CharId}, Package #{PkgId} (Valid until {EndAt})",
            tag,
            userCharacter.UserID,
            userCharacter.CharacterID,
            userCharacter.PackageId,
            userCharacter.EndAt);

        // ✅ Return true để PaymentService biết cần call SaveChangesAsync
        return true;
    }
}