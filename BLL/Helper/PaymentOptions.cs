using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helper
{
    public class PaymentRootOptions
    {
        public string ReturnBaseUrl { get; set; } = string.Empty;
        public MoMoOptions MoMo { get; set; } = new();
        public VNPayOptions VNPay { get; set; } = new();
    }

    public class VNPayOptions
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = "2.1.0";
        public string IpnPath { get; set; } = string.Empty;
        public string ReturnPath { get; set; } = string.Empty;
    }

    public class MoMoOptions
    {
        public string PartnerCode { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string IpnPath { get; set; } = string.Empty;
        public string ReturnPath { get; set; } = string.Empty;
    }
}
