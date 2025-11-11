using System.Collections.Generic;
using BLL.DTO;

namespace BLL.IService;

public interface IPaymentService
{
    Task<PaymentStartResponse> CreateMoMoPaymentAsync(
        decimal amount, int? orderId, int? characterOrderId, CancellationToken ct = default);

    Task<(bool ok, string message)> HandleMoMoIpnAsync(IDictionary<string, string> payload, CancellationToken ct = default);
}
