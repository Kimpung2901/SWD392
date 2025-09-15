using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BookingDetail
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public int DollVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual DollVariant DollVariant { get; set; } = null!;
        public virtual ICollection<BookingEmotion> BookingEmotions { get; set; } = new List<BookingEmotion>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    }
}
