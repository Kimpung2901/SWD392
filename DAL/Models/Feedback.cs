using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public int BookingDetailId { get; set; }
        public int RatingScore { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public virtual BookingDetail BookingDetail { get; set; }
        public virtual User User { get; set; }

    }
}


