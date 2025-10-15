using System.Security.Cryptography;
using System.Text;

namespace BLL.Helper;

public static class MoMoSign
{
    public static string BuildRawToSign(
        string accessKey, long amount, string extraData, string ipnUrl,
        string orderId, string orderInfo, string partnerCode, string returnUrl,
        string requestId, string requestType)
    {
        // Thứ tự alphabet đúng theo tài liệu MoMo
        return
            $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}" +
            $"&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}" +
            $"&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";
    }

    public static string HmacSha256Lower(string raw, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
