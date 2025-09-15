using AutoMapper;
using DAL.DTO;
using DAL.Models;

namespace DAL.Helper
{
    public class Mapping:Profile
    {
        public Mapping() 
        {
            CreateMap<User, GetUserRespone>();
            CreateMap<GetUserRespone, User>();
        }
    }
}
