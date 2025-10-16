using BLL.IService;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

public class FakeMoMoProvider : IPaymentProvider
{
    public string Name => "MoMo";
    private readonly ILogger<FakeMoMoProvider> _logger;
    public FakeMoMoProvider(ILogger<FakeMoMoProvider> logger) => _logger = logger;

    public async Task<Payment> CreatePaymentAsync(DollDbContext db, Payment payment, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);

        var orderId = $"FAKE{DateTime.UtcNow:yyyyMMddHHmmss}{payment.Target_Id:D4}";
        var requestId = Guid.NewGuid().ToString("N");

        payment.Provider = Name;
        payment.Method = "Wallet";
        payment.OrderId = orderId;    // momo orderId
        payment.TransactionId = requestId;  // requestId
        payment.Status = "Pending";

        // ✅ Route mới nằm dưới PaymentController: /api/payment/sandbox/momo
        payment.PayUrl = $"/api/payment/sandbox/momo?orderId={orderId}&amount={payment.Amount}";
        payment.RawResponse = $"{{\"resultCode\":0,\"message\":\"Success(FAKE)\",\"orderId\":\"{orderId}\"}}";

        await db.SaveChangesAsync(ct);
        return payment;
    }

    public async Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> data, CancellationToken ct = default)
    {
        if (!data.TryGetValue("orderId", out var orderId)) return false;

        var p = await db.Payments.FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
        if (p == null) return false;

        // Idempotent
        if (p.Status is "Success" or "Failed") return true;

        var rc = data.TryGetValue("resultCode", out var s) && int.TryParse(s, out var i) ? i : -1;
        if (rc == 0)
        {
            p.Status = "Success";
            p.CompletedAt = DateTime.UtcNow;
        }
        else p.Status = "Failed";

        p.RawResponse = $"IPN(FAKE):{string.Join("&", data.Select(kv => $"{kv.Key}={kv.Value}"))}";
        await db.SaveChangesAsync(ct);
        return true;
    }
}
