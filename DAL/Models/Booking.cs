using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingFree { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
        public virtual ICollection<BookingEmotion> BookingEmotions { get; set; } = new List<BookingEmotion>();
        public virtual ICollection<BookingTracking> BookingTrackings { get; set; } = new List<BookingTracking>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
