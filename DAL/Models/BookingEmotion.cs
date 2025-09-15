using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class BookingEmotion
    {
        public int BookingEmotionId { get; set; }
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public int EmotionId { get; set; }
        public decimal EmotionPrice { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual BookingDetail BookingDetail { get; set; } = null!;
        public virtual Emotion Emotion { get; set; } = null!;

    }
}
