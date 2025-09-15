using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BookingTracking
    {
        public int TrackingId {  get; set; }
        public int BookingId { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime UpdateAt { get; set; }

        public virtual Booking Booking { get; set; } = null!;

    }
}
