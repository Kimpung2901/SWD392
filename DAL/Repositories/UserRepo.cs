using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepo
    {      
            private readonly Swd1DbContext _context;

            public UserRepo(Swd1DbContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }
            public async Task<List<User>> GetAllUsers()
            {
                return await _context.Users.ToListAsync();
            }
            public async Task<User> getUserByUsername(string username)
            {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
        public async Task<User?> getUserByPhone(string PhoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
        }

        public async Task<User?> getUserByGmail(string gmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Gmail == gmail);
        }
         public async Task<User?> getUserById(int Id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == Id);
        }
        public async Task<bool> addUser(User user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> updateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> deleteUser(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }


    }
}
