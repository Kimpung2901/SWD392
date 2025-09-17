using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Doll
    {
        public int DollId { get; set; }
        public string Name { get; set; }
        public string Avatar {  get; set; }
        public decimal BasePrice { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<DollVariant> DollVariants { get; set; } = new List<DollVariant>();


    }
}
