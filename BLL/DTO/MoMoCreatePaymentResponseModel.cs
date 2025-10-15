using System.Text.Json.Serialization;

namespace BLL.DTO;

public class MoMoCreatePaymentResponseModel
{
    [JsonPropertyName("partnerCode")] public string PartnerCode { get; set; } = "";
    [JsonPropertyName("orderId")] public string OrderId { get; set; } = "";
    [JsonPropertyName("requestId")] public string RequestId { get; set; } = "";
    [JsonPropertyName("amount")] public long Amount { get; set; }
    [JsonPropertyName("responseTime")] public long ResponseTime { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; } = "";
    [JsonPropertyName("resultCode")] public int ResultCode { get; set; }
    [JsonPropertyName("payUrl")] public string? PayUrl { get; set; }
    [JsonIgnore] public bool IsSuccess => ResultCode == 0;
}
