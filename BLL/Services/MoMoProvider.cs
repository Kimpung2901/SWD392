using System.Net.Http.Json;
using System.Text.Json;
using BLL.DTO;
using BLL.Helper;
using BLL.IService;
using BLL.Options;
using DAL.Enum;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public class MoMoProvider : IPaymentProvider
{
    public string Name => "MoMo";
    private readonly PaymentRootOptions _opt;
    private readonly HttpClient _http;
    private readonly ILogger<MoMoProvider> _logger;

    public MoMoProvider(HttpClient http, IOptions<PaymentRootOptions> opt, ILogger<MoMoProvider> logger)
    {
        _http = http; _opt = opt.Value; _logger = logger;
    }

    public async Task<Payment> CreatePaymentAsync(DollDbContext db, Payment payment, CancellationToken ct = default)
    {
        var m = _opt.MoMo;
        var total = (long)Math.Round(payment.Amount, MidpointRounding.AwayFromZero);

        var orderId = $"MOMO{DateTime.UtcNow:yyyyMMddHHmmss}{payment.Target_Id:D4}";
        var requestId = Guid.NewGuid().ToString("N");

        var returnUrl = _opt.ReturnBaseUrl.TrimEnd('/') + m.ReturnPath; // FE quay về sau khi user thao tác
        var ipnUrl = _opt.ReturnBaseUrl.TrimEnd('/') + m.IpnPath;    // server-to-server
        var extraData = "";
        var orderInfo = payment.OrderInfo ?? $"Thanh toan {payment.Target_Type}:{payment.Target_Id}";

        // ký HMAC
        var raw = MoMoSign.BuildRawToSign(m.AccessKey, total, extraData, ipnUrl, orderId, orderInfo,
                                          m.PartnerCode, returnUrl, requestId, "captureWallet");
        var signature = MoMoSign.HmacSha256Lower(raw, m.SecretKey);

        var payload = new
        {
            partnerCode = m.PartnerCode,
            accessKey = m.AccessKey,
            requestId,
            amount = total.ToString(),
            orderId,
            orderInfo,
            redirectUrl = returnUrl,
            ipnUrl,
            extraData,
            requestType = "captureWallet",
            signature,
            lang = "vi"
        };

        _logger.LogInformation("[MoMo] Create orderId={OrderId}, amount={Amount}", orderId, total);
        _logger.LogDebug("[MoMo] rawToSign={Raw}", raw);
        _logger.LogDebug("[MoMo] signature={Sig}", signature);

        var res = await _http.PostAsJsonAsync(m.Endpoint, payload, cancellationToken: ct);
        var json = await res.Content.ReadAsStringAsync(ct);
        _logger.LogInformation("[MoMo] Status={Status}, Body={Body}", res.StatusCode, json);

        var momo = JsonSerializer.Deserialize<MoMoCreatePaymentResponseModel>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // cập nhật bản ghi
        payment.Provider = Name;
        payment.Method = "Wallet";
        payment.OrderId = orderId;
        payment.TransactionId = requestId;
        payment.RawResponse = json;

        if (momo?.IsSuccess == true && !string.IsNullOrEmpty(momo.PayUrl))
        {
            payment.PayUrl = momo.PayUrl;   // ← URL trang MoMo (hiển thị QR)
            payment.Status = PaymentStatus.Pending;
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            payment.OrderInfo = $"MoMo Error [{momo?.ResultCode ?? -1}]: {momo?.Message ?? "Unknown"}";
        }

        await db.SaveChangesAsync(ct);
        return payment;
    }

    public async Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> form, CancellationToken ct = default)
    {
        if (!VerifyIpn(form))
        {
            _logger.LogWarning("[MoMo IPN] Signature invalid");
            return false;
        }

        if (!form.TryGetValue("orderId", out var orderId))
            return false;

        var p = await db.Payments.FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
        if (p == null) return false;

        if (p.Status == PaymentStatus.Completed || p.Status == PaymentStatus.Failed) return true; // idempotent

        int rc = -1;
        var hasRc = form.TryGetValue("resultCode", out var rcStr) && int.TryParse(rcStr, out rc);
        p.Status = (hasRc && rc == 0) ? PaymentStatus.Completed : PaymentStatus.Failed;
        if (p.Status == PaymentStatus.Completed) p.CompletedAt = DateTime.UtcNow;

        p.RawResponse = $"IPN:{string.Join("&", form.Select(kv => $"{kv.Key}={kv.Value}"))}";
        await db.SaveChangesAsync(ct);
        return true;
    }

    private bool VerifyIpn(IDictionary<string, string> f)
    {
        f.TryGetValue("accessKey", out var accessKey);
        f.TryGetValue("amount", out var amount);
        f.TryGetValue("extraData", out var extraData);
        f.TryGetValue("orderId", out var orderId);
        f.TryGetValue("orderInfo", out var orderInfo);
        f.TryGetValue("orderType", out var orderType);
        f.TryGetValue("partnerCode", out var partnerCode);
        f.TryGetValue("payType", out var payType);
        f.TryGetValue("requestId", out var requestId);
        f.TryGetValue("responseTime", out var responseTime);
        f.TryGetValue("resultCode", out var resultCode);
        f.TryGetValue("message", out var message);
        f.TryGetValue("transId", out var transId);
        f.TryGetValue("signature", out var sig);

        var raw =
            $"accessKey={accessKey}&amount={amount}&extraData={extraData}&message={message}" +
            $"&orderId={orderId}&orderInfo={orderInfo}&orderType={orderType}&partnerCode={partnerCode}" +
            $"&payType={payType}&requestId={requestId}&responseTime={responseTime}" +
            $"&resultCode={resultCode}&transId={transId}";

        var calc = MoMoSign.HmacSha256Lower(raw, _opt.MoMo.SecretKey);
        return string.Equals(calc, sig, StringComparison.OrdinalIgnoreCase);
    }
}
