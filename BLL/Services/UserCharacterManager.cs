using BLL.Helper;
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

        // Validate required fields
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


        var vietnamNow = DateTimeHelper.GetVietnamTime();
        
        var userCharacter = new UserCharacter
        {
            UserID = characterOrder.UserID,
            CharacterID = characterOrder.CharacterID,
            PackageId = characterOrder.PackageID,
            StartAt = vietnamNow,  
            EndAt = vietnamNow.AddDays(package.DurationDays),  
            AutoRenew = false,
            Status = UserCharacterStatus.Active,
            CreatedAt = vietnamNow
        };

        _db.UserCharacters.Add(userCharacter);

        _logger.LogInformation(
            "[{Tag}] ✅ Created UserCharacter for User #{UserId}, Character #{CharId}, Package #{PkgId}" +
            "\n   📅 Start: {StartAt} (Vietnam Time)" +
            "\n   📅 End: {EndAt} (Vietnam Time)" +
            "\n   ⏱️ Duration: {Days} days",
            tag,
            userCharacter.UserID,
            userCharacter.CharacterID,
            userCharacter.PackageId,
            userCharacter.StartAt.ToString("yyyy-MM-dd HH:mm:ss"),
            userCharacter.EndAt.ToString("yyyy-MM-dd HH:mm:ss"),
            package.DurationDays);

        return true;
    }
}