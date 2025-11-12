using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Helper
{
    public static class MoMoSign
    {
        /// <summary>
        /// Tạo raw string để ký theo đúng thứ tự MoMo yêu cầu.
        /// </summary>
        public static string BuildRawToSign(
            string accessKey,
            long amount,
            string extraData,
            string ipnUrl,
            string orderId,
            string orderInfo,
            string partnerCode,
            string redirectUrl,
            string requestId,
            string requestType)
        {
            return $"accessKey={accessKey}" +
                   $"&amount={amount}" +
                   $"&extraData={extraData}" +
                   $"&ipnUrl={ipnUrl}" +
                   $"&orderId={orderId}" +
                   $"&orderInfo={orderInfo}" +
                   $"&partnerCode={partnerCode}" +
                   $"&redirectUrl={redirectUrl}" +
                   $"&requestId={requestId}" +
                   $"&requestType={requestType}";
        }

        /// <summary>
        /// Tạo HMAC SHA256 chữ ký (lowercase) bằng secret key.
        /// </summary>
        public static string HmacSha256Lower(string rawData, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2")); // lowercase hex
            return sb.ToString();
        }

        /// <summary>
        /// So sánh chữ ký (bất kể hoa/thường)
        /// </summary>
        public static bool VerifySignature(string rawData, string secretKey, string? momoSignature)
        {
            if (string.IsNullOrEmpty(momoSignature)) return false;
            var calc = HmacSha256Lower(rawData, secretKey);
            return string.Equals(calc, momoSignature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
