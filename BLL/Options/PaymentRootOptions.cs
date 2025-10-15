namespace BLL.Options;

public sealed class PaymentRootOptions
{
    public string ReturnBaseUrl { get; set; } = "https://your-public-domain"; // phải PUBLIC để MoMo gọi IPN
    public MoMoOptions MoMo { get; set; } = new();
}

public sealed class MoMoOptions
{
    public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
    public string PartnerCode { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string ReturnPath { get; set; } = "/api/payment/momo/callback";
    public string IpnPath { get; set; } = "/api/payment/momo/ipn";
}
