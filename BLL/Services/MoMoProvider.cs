using System.Net.Http.Json;
using System.Text;
using BLL.Helper;
using BLL.IService;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public class MoMoProvider : IPaymentProvider
{
    public string Name => "MoMo";
    private readonly PaymentRootOptions _opt;
    private readonly HttpClient _http = new();

    public MoMoProvider(IOptions<PaymentRootOptions> opt) => _opt = opt.Value;

    public async Task<Payment> CreatePaymentAsync(DollDbContext db, Payment p, CancellationToken ct = default)
    {
        var momo = _opt.MoMo;
        var orderId = $"M{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{p.PaymentID}";
        var requestId = orderId;

        var body = new Dictionary<string, string>
        {
            ["partnerCode"] = momo.PartnerCode,
            ["accessKey"] = momo.AccessKey,
            ["requestId"] = requestId,
            ["amount"] = ((long)p.Amount).ToString(),
            ["orderId"] = orderId,
            ["orderInfo"] = $"{p.Target_Type}:{p.Target_Id}",
            ["returnUrl"] = _opt.ReturnBaseUrl.TrimEnd('/') + momo.ReturnPath,
            ["notifyUrl"] = _opt.ReturnBaseUrl.TrimEnd('/') + momo.IpnPath,
            ["requestType"] = "captureWallet",
            ["extraData"] = ""
        };

        var raw = $"accessKey={body["accessKey"]}&amount={body["amount"]}&extraData={body["extraData"]}&" +
                  $"orderId={body["orderId"]}&orderInfo={body["orderInfo"]}&partnerCode={body["partnerCode"]}&" +
                  $"redirectUrl={body["returnUrl"]}&ipnUrl={body["notifyUrl"]}&requestId={body["requestId"]}&requestType={body["requestType"]}";
        var signature = CryptoHelper.HmacSha512(raw, momo.SecretKey);

        var payload = new
        {
            partnerCode = momo.PartnerCode,
            accessKey = momo.AccessKey,
            requestId,
            amount = body["amount"],
            orderId,
            orderInfo = body["orderInfo"],
            redirectUrl = body["returnUrl"],
            ipnUrl = body["notifyUrl"],
            requestType = body["requestType"],
            extraData = "",
            signature
        };

        var resp = await _http.PostAsJsonAsync(momo.Endpoint, payload, ct);
        var json = await resp.Content.ReadAsStringAsync(ct);
        p.Provider = "MoMo";
        p.Method = "Wallet";
        p.TransactionId = orderId;
        p.PayUrl = TryGet(json, "payUrl") ?? TryGet(json, "deeplink");
        p.RawResponse = json;
        await db.SaveChangesAsync(ct);
        return p;

        static string? TryGet(string json, string key)
        {
            // tránh thêm dependency; parse rất đơn giản
            var tag = $"\"{key}\":\"";
            var i = json.IndexOf(tag, StringComparison.OrdinalIgnoreCase);
            if (i < 0) return null;
            i += tag.Length;
            var j = json.IndexOf('"', i);
            return j > i ? json.Substring(i, j - i) : null;
        }
    }

    public async Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> q, CancellationToken ct = default)
    {
        var momo = _opt.MoMo;
        // verify signature
        var raw = $"accessKey={momo.AccessKey}&amount={GetValue(q, "amount")}&extraData={GetValue(q, "extraData")}&" +
                  $"message={GetValue(q, "message")}&orderId={GetValue(q, "orderId")}&orderInfo={GetValue(q, "orderInfo")}&" +
                  $"orderType={GetValue(q, "orderType")}&partnerCode={GetValue(q, "partnerCode")}&payType={GetValue(q, "payType")}&" +
                  $"requestId={GetValue(q, "requestId")}&responseTime={GetValue(q, "responseTime")}&resultCode={GetValue(q, "resultCode")}&transId={GetValue(q, "transId")}";
        var sign = CryptoHelper.HmacSha512(raw, momo.SecretKey);
        if (!string.Equals(sign, GetValue(q, "signature"), StringComparison.OrdinalIgnoreCase))
            return false;

        var orderId = GetValue(q, "orderId");
        var payment = await db.Payments.FirstOrDefaultAsync(x => x.TransactionId == orderId, ct);
        if (payment == null) return false;

        payment.Status = GetValue(q, "resultCode") == "0" ? "Success" : "Failed";
        payment.RawResponse = $"IPN:{CryptoHelper.ToQueryStringSorted(q, false)}";
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static string GetValue(IDictionary<string, string> dict, string key)
    {
        return dict.TryGetValue(key, out var value) ? value : string.Empty;
    }
}
