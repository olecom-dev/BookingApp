using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public class RestaurantBooking
    {
       public int ID { get; set; }
        public DateTime BookingTime { get; set; }
       public string BookingCode { get; set; }
        public string TableNumber { get; set; }
        public Decimal Price { get; set; }
        public int Multiplicator { get; set; }
        public Decimal PriceOverall { get { return Price * Multiplicator; } }
        public string PricePrintable { get { return Multiplicator.ToString() + " x " + Price.ToString(); } }
        public string ProductName { get; set; }
    }
}
