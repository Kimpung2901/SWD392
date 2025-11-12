namespace BLL.Options;

public sealed class PaymentRootOptions
{
    public string ReturnBaseUrl { get; set; } = "";
    public string? BackendBaseUrl { get; set; } = "";
    public string? FrontendReturnUrl { get; set; }
    public MoMoOptions MoMo { get; set; } = new();
}

public sealed class MoMoOptions
{
    public string Endpoint { get; set; } = "";
    public string PartnerCode { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string ReturnPath { get; set; } = "";
    public string IpnPath { get; set; } = "";
}
