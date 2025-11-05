using BLL.DTO;
using BLL.IService;
using DAL.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebNameProjectOfSWD.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _repo;

    public PaymentController(IPaymentService paymentService, IPaymentRepository repo)
    {
        _paymentService = paymentService;
        _repo = repo;
    }

    // FE calls -> receives payUrl (MoMo) -> FE redirects user to MoMo QR page
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

        // Ensure absolute URL when provider returns relative path
        var payUrl = result.PayUrl!;
        if (Uri.TryCreate(payUrl, UriKind.Relative, out _))
            payUrl = $"{Request.Scheme}://{Request.Host}{payUrl}";

        return Ok(new { success = true, payUrl, paymentId = result.PaymentId });
    }

    // Return URL from MoMo (for UX), do not finalize status here
    [HttpGet("momo/callback")]
    [AllowAnonymous]
    public IActionResult MoMoCallback([FromQuery] string orderId, [FromQuery] string? resultCode)
        => Redirect($"/pay-result?orderId={orderId}&resultCode={resultCode}");

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

