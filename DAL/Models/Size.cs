using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Size
    {
        public int SizeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DollVariant> DollVariants { get; set; } = new List<DollVariant>();
    }
}
