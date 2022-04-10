using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
   public class Booking
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int RoomID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }                               
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string RoomNumber { get; set; }
        public bool Available { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal PriceOverAll { get; set; }
        public decimal PriceOverAllWithoutTax { get; set; }
    }
}
