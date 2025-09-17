using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.DTO.UserDto;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace BLL.Services.User
{
    public class UserService
    {
        private readonly UserRepo _userRepo;
        private readonly IMapper _mapper;

        public UserService(UserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GetUserRespone>> getAllUser()
        {
            var res = await _userRepo.GetAllUsers();

            var list = new List<GetUserRespone>();
            foreach (var u in res)
            {
                if (!u.IsDelete && string.Equals(u.Status, "active", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(_mapper.Map<GetUserRespone>(u));
                }
            }
            return list;
        }

        public async Task<GetUserRespone?> getUserById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be > 0");

            var user = await _userRepo.getUserById(id);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            if (user.IsDelete)
                throw new UnauthorizedAccessException("User was deleted.");

            if (!string.Equals(user.Status, "active", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("User was inactived.");

            return _mapper.Map<GetUserRespone>(user);
        }

            public async Task<GetUserRespone> updateUser(int userId, UpdateUser rq)

        {
            if (userId <= 0) throw new ArgumentException("Id must be > 0");
            if (rq is null) throw new ArgumentNullException(nameof(rq));


            var user = await _userRepo.getUserById(userId);
            if (user == null) throw new KeyNotFoundException("User not found");


            if (rq.UserName != null && !string.Equals(rq.UserName, user.UserName, StringComparison.Ordinal))
            {
                var exist = await _userRepo.getUserByUsername(rq.UserName);
                if (exist != null && exist.UserId != userId)
                    throw new ArgumentException("Username is already exist");
                user.UserName = rq.UserName.Trim();
            }


            if (rq.Gmail != null && !string.Equals(rq.Gmail, user.Gmail, StringComparison.OrdinalIgnoreCase))
            {
                var exist = await _userRepo.getUserByGmail(rq.Gmail);
                if (exist != null && exist.UserId != userId)
                    throw new ArgumentException("Gmail is already exist");
                user.Gmail = rq.Gmail.Trim();
            }

            if (rq.PhoneNumber != null && !string.Equals(rq.PhoneNumber, user.PhoneNumber, StringComparison.Ordinal))
            {
                var exist = await _userRepo.getUserByPhone(rq.PhoneNumber);
                if (exist != null && exist.UserId != userId)
                    throw new ArgumentException("Phone number is already exist");
                user.PhoneNumber = rq.PhoneNumber.Trim();
            }


            if (rq.Avatar != null) user.Avatar = rq.Avatar;
            if (rq.Gender != null) user.Gender = rq.Gender;
            if (rq.Address != null) user.Address = rq.Address;
            if (rq.DateOfBirth.HasValue) user.DateOfBirth = rq.DateOfBirth.Value;


            if (rq.Password != null)
                user.Password = BCrypt.Net.BCrypt.HashPassword(rq.Password);


            if (rq.IsDelete.HasValue)
            {
                user.IsDelete = rq.IsDelete.Value;
                user.Status = user.IsDelete ? "Inactive" : "Active";
                user.DeletedAt = user.IsDelete ? DateTime.UtcNow : null;
            }

            if (rq.Status != null) user.Status = rq.Status;

            await _userRepo.updateUser(user);
            return _mapper.Map<GetUserRespone>(user);

        }

        public async Task<bool> SoftDeleteUser(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Id must be > 0");
            var user = await _userRepo.getUserById(userId, includeDeleted: true);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (user.IsDelete) return true; 

            return await _userRepo.SoftDelete(user);
        }

        public async Task<bool> RestoreUser(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Id must be > 0");
            var user = await _userRepo.getUserById(userId, includeDeleted: true);
            if (user == null) throw new KeyNotFoundException("User not found");

            if (!user.IsDelete) return true; 
            return await _userRepo.Restore(user);
        }


        public async Task<bool> HardDeleteUser(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Id must be > 0");
            var user = await _userRepo.getUserById(userId, includeDeleted: true);
            if (user == null) throw new KeyNotFoundException("User not found");

            return await _userRepo.HardDelete(user);
        }

        public async Task<bool> AddUser(AddUser rq)
        {
            if (rq == null) throw new ArgumentNullException(nameof(rq));

            if (string.IsNullOrWhiteSpace(rq.UserName))
                throw new ArgumentNullException("Name can not be blank");
            if (string.IsNullOrWhiteSpace(rq.Password))
                throw new ArgumentNullException("Password can not be blank");
            if (string.IsNullOrWhiteSpace(rq.Gmail))
                throw new ArgumentNullException("Gmail can not be blank");
            if (string.IsNullOrWhiteSpace(rq.PhoneNumber))
                throw new ArgumentNullException("Phone number can not be blank");
            if (rq.DateOfBirth == default)
                throw new ArgumentNullException("Date of birth can not be blank");
            if (string.IsNullOrWhiteSpace(rq.Avatar))
                throw new ArgumentNullException("Avatar can not be blank");
            if (string.IsNullOrWhiteSpace(rq.Gender))
                throw new ArgumentNullException("Gender can not be blank");

            if (await _userRepo.getUserByUsername(rq.UserName) != null)
                throw new ArgumentException("Username is already exist");

            if (await _userRepo.getUserByPhone(rq.PhoneNumber) != null)
                throw new ArgumentException("Phone Number is already exist");

            if (await _userRepo.getUserByGmail(rq.Gmail) != null)
                throw new ArgumentException("Gmail is already exist");

            try
            {
                // Hash password
                string encryptPassword = BCrypt.Net.BCrypt.HashPassword(rq.Password);

                var user = _mapper.Map<DAL.Models.User>(rq);

                user.Password = encryptPassword;
                user.Role = string.IsNullOrWhiteSpace(rq.Role) ? "User" : rq.Role.Trim();
                user.Status = "Active";

                if (user.CreateAt == default)
                    user.CreateAt = DateTime.UtcNow; 

                user.IsDelete = false;

                var result = await _userRepo.AddUser(user);
                return result;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error saving data: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Undefined Error: {ex.Message}");
            }
        }
    }
}
