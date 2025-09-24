//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.PortableExecutable;
//using System.Text;
//using System.Threading.Tasks;
//using DAL.Models;
//using Microsoft.EntityFrameworkCore;

//namespace DAL.Repositories
//{
//    public class UserRepo
//    {
//        private readonly Swd1DbContext _context;

//        public UserRepo(Swd1DbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }
//        public async Task<List<User>> GetAllUsers()
//        {
//            return await _context.Users.ToListAsync();
//        }
//        public async Task<User?> getUserById(int id, bool includeDeleted = true)
//        {
//            var query = _context.Users.AsQueryable();
//            if (!includeDeleted)
//            {
//                query = query.Where(u => !u.IsDelete);
//            }

//            return await query.FirstOrDefaultAsync(u => u.UserId == id);
//        }


//        public async Task<User?> getUserByUsername(string Username)
//        {
//            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == Username);
//        }

//        public async Task<User?> getUserByPhone(string PhoneNumber)
//        {
//            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
//        }

//        public async Task<User?> getUserByGmail(string gmail)
//        {
//            return await _context.Users.FirstOrDefaultAsync(u => u.Gmail == gmail);
//        }
//        public async Task<bool> AddUser(User user)
//        {
//            await _context.Users.AddAsync(user);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> updateUser(User user)
//        {
//            _context.Users.Update(user);
//            return await _context.SaveChangesAsync() > 0;
//        }
//        public async Task<bool> SoftDelete(User user)
//        {
//            user.IsDelete = true;
//            user.Status = "Inactive";
//            user.DeletedAt = DateTime.UtcNow;
//            _context.Users.Update(user);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> Restore(User user)
//        {
//            user.IsDelete = false;
//            user.Status = "Active";
//            user.DeletedAt = null;
//            _context.Users.Update(user);
//            return await _context.SaveChangesAsync() > 0;
//        }

//        public async Task<bool> HardDelete(User user)
//        {
//            _context.Users.Remove(user);
//            return await _context.SaveChangesAsync() > 0;
//        }



//    }
//}
