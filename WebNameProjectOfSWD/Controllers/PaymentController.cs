using BLL.DTO;
using BLL.IService;
using BLL.Options;
using DAL.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
        private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _repo;
    private readonly PaymentRootOptions _paymentOptions;

    public PaymentController(
        IPaymentService paymentService,
        IPaymentRepository repo,
        IOptions<PaymentRootOptions> paymentOptions)
    {
        _paymentService = paymentService;
        _repo = repo;
        _paymentOptions = paymentOptions.Value;
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest req, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _paymentService.CreateMoMoPaymentAsync(
            req.Amount,
            req.OrderId,
            req.CharacterOrderId,
            ct);

        if (!result.Success) return BadRequest(new { success = false, message = result.Message });


        var payUrl = result.PayUrl!;
        if (Uri.TryCreate(payUrl, UriKind.Relative, out _))
            payUrl = $"{Request.Scheme}://{Request.Host}{payUrl}";

        return Ok(new { success = true, payUrl, paymentId = result.PaymentId });
    }


     [HttpGet("momo/callback")]
    [AllowAnonymous]
    public IActionResult MoMoCallback([FromQuery] string orderId, [FromQuery] string? resultCode)
    {
        var feReturn = _paymentOptions.FrontendReturnUrl?.Trim();
        

        Console.WriteLine($"[MoMoCallback] FrontendReturnUrl: '{feReturn}'");
        Console.WriteLine($"[MoMoCallback] orderId: {orderId}, resultCode: {resultCode}");
        
        if (!string.IsNullOrEmpty(feReturn))
        {
            var separator = feReturn.Contains('?') ? '&' : '?';
            var redirectUrl = $"{feReturn}{separator}orderId={orderId}&resultCode={resultCode}";
            Console.WriteLine($"[MoMoCallback] Redirecting to: {redirectUrl}");
            return Redirect(redirectUrl);
        }

        var fallbackUrl = $"https://doll-sales-system-fe.vercel.app/pay-result?orderId={orderId}&resultCode={resultCode}";
        Console.WriteLine($"[MoMoCallback] No FrontendReturnUrl, using fallback: {fallbackUrl}");
        return Redirect(fallbackUrl);
    }

    // IPN: finalize payment status
    [HttpPost("momo/ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> MoMoIpn(CancellationToken ct)
    {
        var (ok, message) = await _paymentService.HandleMoMoIpnAsync(Request.Form, ct);
        return Ok(new { resultCode = ok ? 0 : 1, message });
    }

    // FE polls payment status
    [HttpGet("{paymentId:int}")]
    public async Task<IActionResult> GetStatus(int paymentId)
    {
        var p = await _repo.GetByIdAsync(paymentId);
        if (p == null) return NotFound();
        return Ok(new { status = p.Status, orderId = p.OrderId, amount = p.Amount, completedAt = p.CompletedAt });
    }
}

