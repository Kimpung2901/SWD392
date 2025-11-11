using System;
using System.Collections.Generic;
using BLL.DTO;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly DollDbContext _db;
    private readonly IPaymentProvider _momo;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository repo, DollDbContext db, IEnumerable<IPaymentProvider> providers, ILogger<PaymentService> logger)
    {
        _repo = repo; _db = db; _logger = logger;
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

    public async Task<(bool ok, string message)> HandleMoMoIpnAsync(IFormCollection form, CancellationToken ct = default)
    {
        try
        {
            var dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());
            var ok = await _momo.HandleIpnAsync(_db, dict, ct);
            if (ok)
            {
                var payment = await FindPaymentFromIpnAsync(dict, ct);
                if (payment != null)
                {
                    await SyncPaymentTargetAsync(payment, isFinalState: true, ct);
                }
                else
                {
                    _logger.LogWarning("Could not resolve payment from MoMo IPN payload");
                }
            }
            return (ok, ok ? "IPN processed successfully" : "Invalid signature");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IPN error");
            return (false, ex.Message);
        }
    }

    private async Task SyncPaymentTargetAsync(Payment payment, bool isFinalState, CancellationToken ct)
    {
        if (string.Equals(payment.Target_Type, "Order", StringComparison.OrdinalIgnoreCase) && payment.OrderID.HasValue)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderID == payment.OrderID.Value, ct);
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
            var characterOrder = await _db.CharacterOrders.FirstOrDefaultAsync(co => co.CharacterOrderID == payment.CharacterOrderID.Value, ct);
            if (characterOrder == null)
            {
                _logger.LogWarning("CharacterOrder #{CharacterOrderId} not found when syncing payment #{PaymentId}", payment.CharacterOrderID, payment.PaymentID);
                return;
            }

            var changed = false;
            if (isFinalState)
            {
                if (payment.Status == PaymentStatus.Completed)
                {
                    if (characterOrder.Status == CharacterOrderStatus.Pending)
                    {
                        characterOrder.Status = CharacterOrderStatus.Completed;
                        changed = true;
                    }
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
