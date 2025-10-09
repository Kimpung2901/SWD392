using BLL.IService;
using DAL.Models; 
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _svc;
        
        public PaymentController(IPaymentService svc) => _svc = svc;

        // Tạo giao dịch
        [HttpPost("start")]
        public async Task<IActionResult> Start(
            [FromQuery] string provider, 
            [FromQuery] decimal amount,
            [FromQuery] string targetType, 
            [FromQuery] int targetId, 
            [FromQuery] int? orderId, 
            [FromQuery] int? characterOrderId)
        {
            var p = await _svc.StartAsync(provider, amount, targetType, targetId, orderId, characterOrderId);
            return Ok(new 
            { 
                p.PaymentID, 
                p.Provider, 
                p.Status, 
                p.PayUrl, 
                p.TransactionId 
            });
        }

        // MoMo IPN
        [HttpPost("momo/ipn")]
        public async Task<IActionResult> MoMoIpn()
        {
            var form = Request.HasFormContentType ? Request.Form : null;
            var dict = form?.ToDictionary(k => k.Key, v => v.Value.ToString()) ?? new Dictionary<string, string>();
            var ok = await _svc.HandleIpnAsync("MoMo", dict);
            return Ok(new { result = ok ? 0 : 1 });
        }

        // VNPay IPN 
        [HttpGet("vnpay/ipn")]
        public async Task<IActionResult> VNPayIpnGet()
        {
            var dict = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
            var ok = await _svc.HandleIpnAsync("VNPay", dict);
            return Ok(new { RspCode = ok ? "00" : "97", Message = ok ? "Success" : "Invalid signature" });
        }

        [HttpPost("vnpay/ipn")]
        public Task<IActionResult> VNPayIpnPost() => VNPayIpnGet();
    }
}

