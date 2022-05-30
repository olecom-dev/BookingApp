using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BookingApp.Classes
{
   public class Booking
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int RoomID { get; set; }
        public string CustomerCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }                               
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryNameInternational { get; set; }
        public string RoomNumber { get; set; }
        public bool Available { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal PriceOverAll { get; set; }
        public decimal PriceOverAllWithoutTax { get; set; }
        public  bool Printed { get; set; }
        public DateTime Timestamp { get; set; }
        public BitmapImage Image { get; set; }
        public int BillID { get; set; }
        public string ImageUrl { get; set; }
    }
}
