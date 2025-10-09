using System.Security.Cryptography;
using System.Text;
using System.Web;

public static class CryptoHelper
{
    
    public static string ToQueryStringSorted(IDictionary<string, string> dict, bool urlEncode)
    {
        var ordered = dict.Where(kv => !string.IsNullOrEmpty(kv.Value))
                          .OrderBy(kv => kv.Key, StringComparer.Ordinal);
        return string.Join("&", ordered.Select(kv =>
            $"{kv.Key}={(urlEncode ? HttpUtility.UrlEncode(kv.Value) : kv.Value)}"));
    }

    public static string HmacSha512(string raw, string secret)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret ?? string.Empty));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw ?? string.Empty));
        return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
    }
}
