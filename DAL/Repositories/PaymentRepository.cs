using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly DollDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    public PaymentRepository(DollDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task<Payment?> GetByIdAsync(int id) => await _db.Payments.FindAsync(id);

    public async Task AddAsync(Payment p)
    {
        _db.Payments.Add(p);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payment p)
    {
        if (_db.Entry(p).State == EntityState.Detached) _db.Payments.Attach(p);
        _db.Entry(p).State = EntityState.Modified;
        await _unitOfWork.SaveChangesAsync();
    }
}
