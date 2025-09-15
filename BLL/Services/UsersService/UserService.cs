using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using DAL.DTO;
using AutoMapper;

namespace BLL.Services.User
{
       public class UserService
    {
        private readonly UserRepo _userRepo;
        private readonly IMapper _mapper ;
        

        public UserService(UserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<List<GetUserRespone>> getAllUser()
        {
            var res = await _userRepo.GetAllUsers();
            List<GetUserRespone> list = new List<GetUserRespone>();
            foreach (var listItem in res)
            {
                if (listItem.IsDelete == false && listItem.Status.ToLower() == "active")
                {
                    var rs = _mapper.Map<GetUserRespone>(listItem);
                    list.Add(rs);
                }
            }
            return list;
        }
        public async Task<GetUserRespone?> getUserById(int Id)
        {
            var user = await _userRepo.getUserById(Id);
            if (string.IsNullOrWhiteSpace(Id.ToString()))
            {
                throw new Exception("Id can not blank");
            }
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            if (user.IsDelete == true)
            {
                throw new UnauthorizedAccessException("User was deleted.");
            }
            else if (user.Status.ToLower() != "active")
            {
                throw new UnauthorizedAccessException("User was inactived.");
            }
            var userRes = _mapper.Map<GetUserRespone>(user);
            return userRes;

        }
    }

}
