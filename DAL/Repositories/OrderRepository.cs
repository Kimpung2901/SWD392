using DAL.IRepo;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DollDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    public OrderRepository(DollDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public IQueryable<Order> GetQueryable()
    {
        return _db.Orders.AsQueryable();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _db.Orders
            .FirstOrDefaultAsync(o => o.OrderID == id);
    }

    public async Task<List<Order>> GetByUserIdAsync(int userId)
    {
        return await _db.Orders
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Order entity)
    {
        _db.Orders.Add(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order entity)
    {
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _db.Orders.Attach(entity);
        }

        _db.Entry(entity).State = EntityState.Modified;
        await _unitOfWork.SaveChangesAsync(); // ✅ Save cả Order + OwnedDoll (nếu có)
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Orders.FindAsync(id);
        if (entity != null)
        {
            _db.Orders.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
