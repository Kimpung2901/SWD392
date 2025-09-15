using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class DollVariant
    {
        public int DollVariantId { get; set; }
        public int DollId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public decimal Price { get; set; }
        public string Stock { get; set; }

        public virtual Doll Doll { get; set; } = null!;
        public virtual Color Color { get; set; } = null!;
        public virtual Size Size { get; set; } = null!;
        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    }
}
  

