using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.DollTypeDTO
{
    public class DollTypeDto
    {
        public int DollTypeID { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Create_at { get; set; }
        public string Image { get; set; } = "";
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
