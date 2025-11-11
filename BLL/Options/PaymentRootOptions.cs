namespace BLL.Options;

public sealed class PaymentRootOptions
{
    public string ReturnBaseUrl { get; set; } = "https://dollaistore-api-dxdggjazgpckh2cc.japaneast-01.azurewebsites.net";
    public string? FrontendReturnUrl { get; set; }   
    public MoMoOptions MoMo { get; set; } = new();
}

public sealed class MoMoOptions
{
    public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
    public string PartnerCode { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string ReturnPath { get; set; } = "/api/payments/momo/callback";
    public string IpnPath { get; set; } = "/api/payments/momo/ipn";
}
