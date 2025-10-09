using BLL.Helper;
using BLL.IService;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public class VNPayProvider : IPaymentProvider
{
    public string Name => "VNPay";
    private readonly PaymentRootOptions _opt;

    public VNPayProvider(IOptions<PaymentRootOptions> opt) => _opt = opt.Value;

    public async Task<Payment> CreatePaymentAsync(DollDbContext db, Payment p, CancellationToken ct = default)
    {
        var v = _opt.VNPay;

        // TxnRef numeric-only để tránh lỗi format
        var txnRef = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + p.PaymentID.ToString("D3");
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        var dict = new Dictionary<string, string>
        {
            ["vnp_Version"] = v.ApiVersion,             // 2.1.0
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = v.TmnCode,                // <== phải là mã thật
            ["vnp_Amount"] = ((long)(p.Amount * 100)).ToString(), // VNPay yêu cầu x100
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"Order {p.Target_Id}",
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_IpAddr"] = "127.0.0.1",
            ["vnp_CreateDate"] = now,
            ["vnp_ExpireDate"] = DateTime.UtcNow.AddMinutes(15).ToString("yyyyMMddHHmmss"),
            ["vnp_ReturnUrl"] = _opt.ReturnBaseUrl.TrimEnd('/') + v.ReturnPath,
            ["vnp_NotifyUrl"] = _opt.ReturnBaseUrl.TrimEnd('/') + v.IpnPath
        };

        // 1) Ký trên RAW (không encode) theo thứ tự key
        var signData = CryptoHelper.ToQueryStringSorted(dict, urlEncode: false);
        var secureHash = CryptoHelper.HmacSha512(signData, v.HashSecret);

        // 2) Tạo URL: tham số ENCODED + secure hash
        var queryEncoded = CryptoHelper.ToQueryStringSorted(dict, urlEncode: true);
        var payUrl = $"{v.BaseUrl}?{queryEncoded}&vnp_SecureHashType=HMACSHA512&vnp_SecureHash={secureHash}";

        p.Provider = "VNPay";
        p.Method = "ATM";
        p.TransactionId = txnRef;
        p.PayUrl = payUrl;
        await db.SaveChangesAsync(ct);
        return p;
    }


    public async Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> q, CancellationToken ct = default)
    {
        var v = _opt.VNPay;
        // tách chữ ký
        var received = q.TryGetValue("vnp_SecureHash", out var receivedValue) ? receivedValue : null;
        var dict = q.Where(kv => kv.Key.StartsWith("vnp_") && kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                    .ToDictionary(k => k.Key, v => v.Value);
        var raw = CryptoHelper.ToQueryStringSorted(dict, false);
        var calc = CryptoHelper.HmacSha512(raw, v.HashSecret);
        if (!string.Equals(calc, received, StringComparison.OrdinalIgnoreCase)) return false;

        var txnRef = q.TryGetValue("vnp_TxnRef", out var txnRefValue) ? txnRefValue : null;
        var code = q.TryGetValue("vnp_ResponseCode", out var codeValue) ? codeValue : null;

        var payment = await db.Payments.FirstOrDefaultAsync(x => x.TransactionId == txnRef, ct);
        if (payment == null) return false;

        payment.Status = code == "00" ? "Success" : "Failed";
        payment.RawResponse = $"IPN:{CryptoHelper.ToQueryStringSorted(q, false)}";
        await db.SaveChangesAsync(ct);
        return true;
    }
}
