using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using BLL.DTO;
using BLL.IService;
using BLL.Options;
using DAL.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        var fallbackUrl =$"https://doll-sales-system-fe.vercel.app/payment-result?orderId={orderId}&resultCode={resultCode}";

        Console.WriteLine($"[MoMoCallback] No FrontendReturnUrl, using fallback: {fallbackUrl}");
        return Redirect(fallbackUrl);
    }

    // IPN: finalize payment status
    [HttpPost("momo/ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> MoMoIpn(CancellationToken ct)
    {
        IDictionary<string, string> payload;

        if (Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync(ct);
            payload = form.ToDictionary(k => k.Key, v => v.Value.ToString());
        }
        else
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                return BadRequest(new { resultCode = 1, message = "IPN payload is empty" });
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var dict = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.ValueKind switch
                    {
                        JsonValueKind.String => prop.Value.GetString() ?? string.Empty,
                        JsonValueKind.Number => prop.Value.GetRawText(),
                        JsonValueKind.True => bool.TrueString.ToLowerInvariant(),
                        JsonValueKind.False => bool.FalseString.ToLowerInvariant(),
                        JsonValueKind.Null => string.Empty,
                        _ => prop.Value.GetRawText()
                    };
                }

                payload = dict;
            }
            catch (JsonException)
            {
                return BadRequest(new { resultCode = 1, message = "IPN payload is not valid JSON" });
            }
        }

        if (payload.Count == 0)
        {
            return BadRequest(new { resultCode = 1, message = "IPN payload is empty" });
        }

        var (ok, message) = await _paymentService.HandleMoMoIpnAsync(payload, ct);
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

