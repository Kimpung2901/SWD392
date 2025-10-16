using DAL.Models;

namespace BLL.IService;

public interface IPaymentProvider
{
    string Name { get; }
    Task<Payment> CreatePaymentAsync(DollDbContext db, Payment payment, CancellationToken ct = default);
    Task<bool> HandleIpnAsync(DollDbContext db, IDictionary<string, string> form, CancellationToken ct = default);
}
