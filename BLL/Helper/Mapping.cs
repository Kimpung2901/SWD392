using AutoMapper;
using BLL.DTO.UserDto;
using DAL.Models;

namespace BLL.Helper
{
    public class Mapping:Profile
    {
        public Mapping() 
        {
            CreateMap<User, GetUserRespone>();
            CreateMap<GetUserRespone, User>();
            CreateMap<AddUser, User>();

        }
    }
}
