using BLL.IService;
using DAL.Enum;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class OwnedDollManager : IOwnedDollManager
{
    private readonly DollDbContext _db;
    private readonly ILogger<OwnedDollManager> _logger;

    public OwnedDollManager(DollDbContext db, ILogger<OwnedDollManager> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> EnsureOwnedDollForOrderAsync(Order order, string? contextTag = null)
    {
        var tag = string.IsNullOrWhiteSpace(contextTag) ? nameof(OwnedDollManager) : contextTag;

        if (!order.UserID.HasValue || !order.DollVariantID.HasValue)
        {
            _logger.LogWarning(
                "[{Tag}] Order #{OrderId} cannot create OwnedDoll (UserID={UserId}, VariantID={VariantId})",
                tag,
                order.OrderID,
                order.UserID,
                order.DollVariantID);
            return false;
        }

        var exists = await _db.OwnedDolls
            .FirstOrDefaultAsync(od =>
                od.UserID == order.UserID.Value &&
                od.DollVariantID == order.DollVariantID.Value &&
                od.Status == OwnedDollStatus.Active);

        if (exists != null)
        {
            _logger.LogInformation(
                "[{Tag}] OwnedDoll already exists for User #{UserId}, Variant #{VariantId}",
                tag,
                order.UserID.Value,
                order.DollVariantID.Value);
            return false;
        }

        var now = DateTime.UtcNow;
        var serialCode = $"DOLL{now:yyyyMMddHHmmss}{order.OrderID:D6}";

        var ownedDoll = new OwnedDoll
        {
            UserID = order.UserID.Value,
            DollVariantID = order.DollVariantID.Value,
            SerialCode = serialCode,
            Status = OwnedDollStatus.Active,
            Acquired_at = now,
            Expired_at = now.AddYears(10)
        };

        _db.OwnedDolls.Add(ownedDoll);

        _logger.LogInformation(
            "[{Tag}] Created OwnedDoll {SerialCode} for User #{UserId}, Variant #{VariantId}",
            tag,
            serialCode,
            ownedDoll.UserID,
            ownedDoll.DollVariantID);

        return true;
    }
}
