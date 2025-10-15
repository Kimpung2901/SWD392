using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly DollDbContext _db;
    public PaymentRepository(DollDbContext db) => _db = db;

    public async Task<Payment?> GetByIdAsync(int id) => await _db.Payments.FindAsync(id);

    public async Task AddAsync(Payment p)
    {
        _db.Payments.Add(p);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment p)
    {
        if (_db.Entry(p).State == EntityState.Detached) _db.Payments.Attach(p);
        _db.Entry(p).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }
}
