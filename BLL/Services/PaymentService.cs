using System;
using BLL.DTO;
using BLL.IService;
using DAL.Enum;
using DAL.IRepo;
using DAL.Models;
using Microsoft.AspNetCore.Http;
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
            return (ok, ok ? "IPN processed successfully" : "Invalid signature");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IPN error");
            return (false, ex.Message);
        }
    }
}
