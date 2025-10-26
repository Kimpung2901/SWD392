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

    public async Task<PaymentStartResponse> CreateMoMoPaymentAsync(decimal amount, string targetType, int targetId, int? orderId, int? characterOrderId, CancellationToken ct = default)
    {
        try
        {
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
                OrderInfo = $"Thanh toan {targetType} #{targetId}",
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(p);                    // có PaymentID ngay
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
