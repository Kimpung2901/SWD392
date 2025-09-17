using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO
{
    public class GetUserRespone
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime CreateAt { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
       
    }
}
