using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public class AllRestaurantBookings
    {
        public int ID { get; set; }
        public string Text { get; set; }   
        public DateTime Timestamp { get; set; }   
        public bool Printed { get; set; }
    }
}
