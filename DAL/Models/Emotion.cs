using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Emotion
    {
        public int EmotionId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string  Description { get; set; }
        public decimal Price { get; set; }

        public virtual ICollection<BookingEmotion> BookingEmotions { get; set; } = new List<BookingEmotion>();
    }
}
