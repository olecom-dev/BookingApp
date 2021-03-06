using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    static class GlobalVariables
    {
        public static  string UserID { get; set; }
        public static DateTime LastLogin { get; set; }
        public static Decimal TaxRate { get; set; }
        public static List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        public static string TableNumber { get; set; }
        public static Decimal PriceComplete { get; set; }
    }
}
