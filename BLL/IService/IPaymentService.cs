using BLL.DTO;
using Microsoft.AspNetCore.Http;

namespace BLL.IService;

public interface IPaymentService
{
    Task<PaymentStartResponse> CreateMoMoPaymentAsync(
        decimal amount, string targetType, int targetId, int? orderId, int? characterOrderId, CancellationToken ct = default);

    Task<(bool ok, string message)> HandleMoMoIpnAsync(IFormCollection form, CancellationToken ct = default);
}
