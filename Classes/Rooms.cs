using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BookingApp.Classes
{
    public class Rooms
    {
        public int ID { get; set; }
        public string RoomNumber { get; set; }
        public int NumberOfBeds { get; set; }
        public decimal PricePerNight { get; set; }
        public string RoomSize { get; set; }
        public string ImageUrl { get; set; }
        public byte[] SourceImage { get; set; }
        public bool Available { get; set; }
        public BitmapImage Image { get; set; }
        public string Description { get; set; }

    }
}
