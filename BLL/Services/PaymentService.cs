using BLL.IService;
using DAL.Models;
using DAL.IRepo;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq; // Add this using directive at the top

namespace BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repo;
    private readonly DollDbContext _db;
    private readonly IPaymentProvider _momoProvider;
    private readonly IPaymentProvider _vnpayProvider;

    // Replace all instances of 'ProviderName' with 'Name' to match the IPaymentProvider interface
    public PaymentService(
        IPaymentRepository repo,
        DollDbContext db,
        IEnumerable<IPaymentProvider> providers)
    {
        _repo = repo;
        _db = db;
        _momoProvider = providers.FirstOrDefault(p => p.Name == "MoMo")!;
        _vnpayProvider = providers.FirstOrDefault(p => p.Name == "VNPay")!;
    }

    public async Task<Payment> StartAsync(
        string provider,
        decimal amount,
        string targetType,
        int targetId,
        int? orderId,
        int? characterOrderId)
    {
        var payment = new Payment
        {
            Provider = provider,
            Amount = amount,
            Currency = "VND",
            Status = "Pending",
            Target_Type = targetType,
            Target_Id = targetId,
            OrderID = orderId,
            CharacterOrderID = characterOrderId,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(payment);
        await _repo.SaveChangesAsync();

        // Tạo payment URL
        var selectedProvider = provider == "MoMo" ? _momoProvider : _vnpayProvider;
        await selectedProvider.CreatePaymentAsync(_db, payment);

        return payment;
    }

    public async Task<bool> HandleIpnAsync(string provider, IDictionary<string, string> query)
    {
        var selectedProvider = provider == "MoMo" ? _momoProvider : _vnpayProvider;
            return await selectedProvider.HandleIpnAsync(_db, query);
    }
}
