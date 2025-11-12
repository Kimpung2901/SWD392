using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.DTO.NotificationDto;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly DollDbContext _db;
    private readonly IPaymentProvider _momo;
    private readonly IOwnedDollManager _ownedDollManager;
    private readonly IUserCharacterManager _userCharacterManager;
    private readonly INotificationService _notificationService; // ✅ THÊM
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository repo,
        DollDbContext db,
        IEnumerable<IPaymentProvider> providers,
        IOwnedDollManager ownedDollManager,
        IUserCharacterManager userCharacterManager,
        INotificationService notificationService, // ✅ THÊM
        ILogger<PaymentService> logger)
    {
        _repo = repo;
        _db = db;
        _ownedDollManager = ownedDollManager;
        _userCharacterManager = userCharacterManager;
        _notificationService = notificationService; // ✅ THÊM
        _logger = logger;
        _momo = providers.First(p => p.Name == "MoMo");
    }

    public async Task<PaymentStartResponse> CreateMoMoPaymentAsync(decimal amount, int? orderId, int? characterOrderId, CancellationToken ct = default)
    {
        try
        {
            var hasOrder = orderId.HasValue;
            var hasCharacterOrder = characterOrderId.HasValue;

            if (!hasOrder && !hasCharacterOrder)
                throw new ArgumentException("Either orderId or characterOrderId must be provided.");

            if (hasOrder && hasCharacterOrder)
                throw new ArgumentException("Only one of orderId or characterOrderId can be set per payment.");

            var targetType = hasOrder ? "Order" : "CharacterOrder";
            var targetId = hasOrder ? orderId!.Value : characterOrderId!.Value;
            var orderInfo = hasOrder
                ? $"Thanh toan Order #{targetId}"
                : $"Thanh toan CharacterOrder #{targetId}";

            var p = new Payment
            {
                Provider = "MoMo",
                Method = "Wallet",
                Amount = amount,
                Status = PaymentStatus.Pending,
                Target_Type = targetType,
                Target_Id = targetId,
                OrderID = orderId,
                CharacterOrderID = characterOrderId,
                OrderInfo = orderInfo,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(p);                    // instant PaymentID
            await _momo.CreatePaymentAsync(_db, p, ct); // fill PayUrl, OrderId, TransactionId
            await SyncPaymentTargetAsync(p, isFinalState: false, ct);

            return new PaymentStartResponse
            {
                PaymentId = p.PaymentID,
                PayUrl = p.PayUrl,
                Success = !string.IsNullOrEmpty(p.PayUrl),
                Message = string.IsNullOrEmpty(p.PayUrl) ? $"Create PayUrl failed: {p.RawResponse}" : "OK"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create MoMo payment error");
            return new PaymentStartResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<(bool ok, string message)> HandleMoMoIpnAsync(IDictionary<string, string> payload, CancellationToken ct = default)
    {
        try
        {
            if (payload == null || payload.Count == 0)
            {
                return (false, "IPN payload is empty");
            }

            var normalized = payload is Dictionary<string, string> existing
                ? new Dictionary<string, string>(existing, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(payload, StringComparer.OrdinalIgnoreCase);

            var hasSignature = normalized.ContainsKey("signature") && !string.IsNullOrEmpty(normalized["signature"]);
            
            Payment? payment = null;
            bool ok;
            string message;
            
            if (hasSignature)
            {
                ok = await _momo.HandleIpnAsync(_db, normalized, ct);
                message = ok ? "IPN processed successfully" : "Invalid signature";
                if (ok)
                {
                    payment = await FindPaymentFromIpnAsync(normalized, ct);
                }
            }
            else
            {
                _logger.LogWarning("[IPN] Processing callback simulation without signature verification");
                
                if (!normalized.TryGetValue("orderId", out var orderId))
                    return (false, "Missing orderId");

                var resultCode = NormalizeResultCode(normalized.TryGetValue("resultCode", out var rc) ? rc : null);
                var helperResult = await UpdatePaymentStatusFromCallbackAsync(orderId, resultCode, "CallbackSimulation", ct);
                ok = helperResult.ok;
                message = helperResult.message;
                payment = helperResult.payment;
            }

            if (ok && payment != null)
            {
                _logger.LogWarning("[IPN] Syncing payment target for payment #{PaymentId}", payment.PaymentID);
                await SyncPaymentTargetAsync(payment, isFinalState: true, ct);
            }
            else if (ok)
            {
                _logger.LogWarning("Could not resolve payment from MoMo IPN payload");
            }
            
            return (ok, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IPN error");
            return (false, ex.Message);
        }
    }

    public async Task<(bool ok, string message)> HandleMoMoCallbackAsync(string orderId, string? resultCode, CancellationToken ct = default)
    {
        var normalizedCode = NormalizeResultCode(resultCode);
        _logger.LogWarning("[MoMoCallback] orderId: {OrderId}, resultCode: {ResultCode}", orderId, normalizedCode);

        var helperResult = await UpdatePaymentStatusFromCallbackAsync(orderId, normalizedCode, "Callback", ct);
        if (helperResult.ok && helperResult.payment != null)
        {
            _logger.LogWarning("[MoMoCallback] Syncing payment target for payment #{PaymentId}", helperResult.payment.PaymentID);
            await SyncPaymentTargetAsync(helperResult.payment, isFinalState: true, ct);
        }

        return (helperResult.ok, helperResult.message);
    }

    private async Task SyncPaymentTargetAsync(Payment payment, bool isFinalState, CancellationToken ct)
    {
        if (string.Equals(payment.Target_Type, "Order", StringComparison.OrdinalIgnoreCase) && payment.OrderID.HasValue)
        {
            var order = await _db.Orders
                .Include(o => o.DollVariant)
                .FirstOrDefaultAsync(o => o.OrderID == payment.OrderID.Value, ct);
                
            if (order == null)
            {
                _logger.LogWarning("Order #{OrderId} not found when syncing payment #{PaymentId}", payment.OrderID, payment.PaymentID);
                return;
            }

            var changed = false;
            if (order.PaymentID != payment.PaymentID)
            {
                order.PaymentID = payment.PaymentID;
                changed = true;
            }

            if (isFinalState)
            {
                if (payment.Status == PaymentStatus.Completed)
                {
                    if (order.Status == OrderStatus.Pending)
                    {
                        order.Status = OrderStatus.Processing;
                        changed = true;
                        _logger.LogInformation("[Payment] Order #{OrderId} moved to Processing after payment", order.OrderID);
                        
                        // ✅ GỬI NOTIFICATION CHO DOLLORDER
                        await SendOrderPaymentSuccessNotificationAsync(order, payment, ct);
                    }
                }
                else if (payment.Status == PaymentStatus.Failed ||
                         payment.Status == PaymentStatus.Cancelled ||
                         payment.Status == PaymentStatus.Refunded)
                {
                    if (order.Status == OrderStatus.Processing)
                    {
                        order.Status = OrderStatus.Pending;
                        changed = true;
                        _logger.LogInformation("[Payment] Order #{OrderId} reverted to Pending", order.OrderID);
                    }
                }
            }

            if (changed)
            {
                await _db.SaveChangesAsync(ct);
            }
        }
        else if (string.Equals(payment.Target_Type, "CharacterOrder", StringComparison.OrdinalIgnoreCase) && payment.CharacterOrderID.HasValue)
        {
            var characterOrder = await _db.CharacterOrders
                .Include(co => co.Package)
                .Include(co => co.Character)
                .FirstOrDefaultAsync(co => co.CharacterOrderID == payment.CharacterOrderID.Value, ct);
                
            if (characterOrder == null)
            {
                _logger.LogWarning("CharacterOrder #{CharacterOrderId} not found when syncing payment #{PaymentId}", 
                    payment.CharacterOrderID, payment.PaymentID);
                return;
            }

            var changed = false;
            if (isFinalState)
            {
                if (payment.Status == PaymentStatus.Completed)
                {
                    if (characterOrder.Status != CharacterOrderStatus.Completed)
                    {
                        characterOrder.Status = CharacterOrderStatus.Completed;
                        changed = true;
                        _logger.LogInformation("[Payment] CharacterOrder #{OrderId} marked as Completed", 
                            characterOrder.CharacterOrderID);
                    }

                    var userCharCreated = await _userCharacterManager.EnsureUserCharacterForOrderAsync(
                        characterOrder,
                        "PaymentService.SyncPaymentTarget");
                    changed |= userCharCreated;
                    
                    // ✅ GỬI NOTIFICATION CHO CHARACTERORDER
                    await SendCharacterOrderPaymentSuccessNotificationAsync(characterOrder, payment, ct);
                }
                else if (payment.Status == PaymentStatus.Failed ||
                         payment.Status == PaymentStatus.Cancelled ||
                         payment.Status == PaymentStatus.Refunded)
                {
                    if (characterOrder.Status == CharacterOrderStatus.Completed)
                    {
                        characterOrder.Status = CharacterOrderStatus.Pending;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                await _db.SaveChangesAsync(ct);
            }
        }
    }

    // ✅ METHOD MỚI: Gửi notification cho DollOrder
    private async Task SendOrderPaymentSuccessNotificationAsync(Order order, Payment payment, CancellationToken ct)
    {
        if (!order.UserID.HasValue)
        {
            _logger.LogWarning("[Notification] Order #{OrderId} has no UserID, skipping notification", order.OrderID);
            return;
        }

        try
        {
            var variantName = order.DollVariant?.Name ?? "Doll";
            
            var notificationDto = new SendNotificationDto
            {
                UserId = order.UserID.Value,
                Title = "🎉 Thanh toán thành công!",
                Body = $"Đơn hàng #{order.OrderID} ({variantName}) đã được thanh toán. Chúng tôi sẽ xử lý và giao hàng sớm nhất!",
                Data = new Dictionary<string, string>
                {
                    { "type", "order_payment_success" },
                    { "orderId", order.OrderID.ToString() },
                    { "paymentId", payment.PaymentID.ToString() },
                    { "amount", payment.Amount.ToString("N0") },
                    { "status", "processing" }
                }
            };

            var result = await _notificationService.SendAsync(notificationDto, ct);
            
            if (!string.IsNullOrEmpty(result.MessageId))
            {
                _logger.LogInformation(
                    "[Notification] ✅ Sent payment success notification for Order #{OrderId} to User #{UserId}. MessageId: {MessageId}",
                    order.OrderID, order.UserID.Value, result.MessageId);
            }
            else
            {
                _logger.LogWarning(
                    "[Notification] ⚠️ Notification saved to DB but push failed for Order #{OrderId}",
                    order.OrderID);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[Notification] ❌ Failed to send notification for Order #{OrderId}", 
                order.OrderID);
            // Không throw exception để không ảnh hưởng đến flow chính
        }
    }

    // ✅ METHOD MỚI: Gửi notification cho CharacterOrder
    private async Task SendCharacterOrderPaymentSuccessNotificationAsync(CharacterOrder characterOrder, Payment payment, CancellationToken ct)
    {
        if (characterOrder.UserID <= 0)
        {
            _logger.LogWarning("[Notification] CharacterOrder #{OrderId} has no UserID, skipping notification", characterOrder.CharacterOrderID);
            return;
        }

        try
        {
            var characterName = characterOrder.Character?.Name ?? "Character";
            var packageName = characterOrder.Package?.Name ?? "Package";
            
            var notificationDto = new SendNotificationDto
            {
                UserId = characterOrder.UserID,
                Title = "🎊 Mua character thành công!",
                Body = $"Bạn đã sở hữu {characterName} ({packageName})! Hãy bắt đầu trò chuyện ngay!",
                Data = new Dictionary<string, string>
                {
                    { "type", "character_payment_success" },
                    { "characterOrderId", characterOrder.CharacterOrderID.ToString() },
                    { "characterId", characterOrder.CharacterID.ToString() },
                    { "packageId", characterOrder.PackageID.ToString() },
                    { "paymentId", payment.PaymentID.ToString() },
                    { "amount", payment.Amount.ToString("N0") }
                }
            };

            var result = await _notificationService.SendAsync(notificationDto, ct);
            
            if (!string.IsNullOrEmpty(result.MessageId))
            {
                _logger.LogInformation(
                    "[Notification] ✅ Sent payment success notification for CharacterOrder #{OrderId} to User #{UserId}. MessageId: {MessageId}",
                    characterOrder.CharacterOrderID, characterOrder.UserID, result.MessageId);
            }
            else
            {
                _logger.LogWarning(
                    "[Notification] ⚠️ Notification saved to DB but push failed for CharacterOrder #{OrderId}",
                    characterOrder.CharacterOrderID);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[Notification] ❌ Failed to send notification for CharacterOrder #{OrderId}", 
                characterOrder.CharacterOrderID);
            // Không throw exception
        }
    }

    private async Task<(bool ok, string message, Payment? payment)> UpdatePaymentStatusFromCallbackAsync(string orderId, int resultCode, string sourceTag, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return (false, "Missing orderId", null);
        }

        var payment = await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
        if (payment == null)
        {
            return (false, $"Payment not found for orderId: {orderId}", null);
        }

        if (payment.Status == PaymentStatus.Completed || payment.Status == PaymentStatus.Failed)
        {
            _logger.LogWarning("[{Source}] Payment #{PaymentId} already finalized with status {Status}", sourceTag, payment.PaymentID, payment.Status);
            return (true, "Payment already finalized", payment);
        }

        payment.Status = resultCode == 0 ? PaymentStatus.Completed : PaymentStatus.Failed;
        if (payment.Status == PaymentStatus.Completed)
        {
            payment.CompletedAt = DateTime.UtcNow;
        }

        payment.RawResponse = $"{sourceTag}:orderId={orderId}&resultCode={resultCode}";
        await _db.SaveChangesAsync(ct);

        _logger.LogWarning("[{Source}] Payment #{PaymentId} updated to {Status}", sourceTag, payment.PaymentID, payment.Status);
        return (true, "Payment status updated", payment);
    }

    private static int NormalizeResultCode(string? resultCode)
    {
        return int.TryParse(resultCode, out var code) ? code : -1;
    }

    private async Task<Payment?> FindPaymentFromIpnAsync(IDictionary<string, string> payload, CancellationToken ct)
    {
        if (payload.TryGetValue("orderId", out var momoOrderId) && !string.IsNullOrWhiteSpace(momoOrderId))
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.OrderId == momoOrderId, ct);
            if (payment != null) return payment;
        }

        if (payload.TryGetValue("requestId", out var requestId) && !string.IsNullOrWhiteSpace(requestId))
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.TransactionId == requestId, ct);
            if (payment != null) return payment;
        }

        if (payload.TryGetValue("paymentId", out var paymentIdValue) && int.TryParse(paymentIdValue, out var paymentId))
        {
            var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentID == paymentId, ct);
            if (payment != null) return payment;
        }

        return null;
    }
}


