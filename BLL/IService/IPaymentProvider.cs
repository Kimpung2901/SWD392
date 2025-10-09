using DAL.Models;

namespace BLL.IService;

public interface IPaymentProvider
{
    string Name { get; } // "MoMo" | "VNPay"

    Task<Payment> CreatePaymentAsync(DollDbContext db, Payment payment, CancellationToken ct = default);

    Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> query, CancellationToken ct = default);
}
