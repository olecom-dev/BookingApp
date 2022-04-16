using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RoomsPage : Page
    {
        public string ConnectionString = (App.Current as App).ConnectionString;
        private List<Rooms> rooms = new List<Rooms>();
        private List<Booking> bookings = new List<Booking>();
        List<Rooms> r = null;
        public RoomsPage()
        {
            this.InitializeComponent();
         //   asbSearch.ItemsSource = rooms;  

            
        }
        public class Bookings
        {
           public DateTime StartDate { get; set; }
           public DateTime EndDate { get; set; }
            public String RoomNumber { get; set; }
            
        }
        
        protected  async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            rooms = await GetRooms();
            foreach (var item in rooms)
            {
                // SetAvailability(true, item.RoomNumber);
                bookings.AddRange(GetBookings(item.RoomNumber));
               
            }
            List<Bookings> b = bookings.Select(i => new Bookings { StartDate = i.StartDate, EndDate = i.EndDate, RoomNumber = i.RoomNumber }).ToList();
            {

                foreach (var t in b)
                {
                    if (DateTime.Now <= t.EndDate && DateTime.Now >= t.StartDate)
                    {
                        SetAvailability(false, t.RoomNumber);
                    }
                }
            }
            rooms.Sort(delegate (Rooms x, Rooms y) {
                return x.RoomNumber.CompareTo(y.RoomNumber);
            });
            GridView1.ItemsSource = rooms;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {   
             
   /*         rooms = GetRooms();
            foreach(var item in rooms)
            {
               // SetAvailability(true, item.RoomNumber);
                boo.AddRange(GetBookings(item.RoomNumber));
            }
            List<Bookings> b = boo.Select(i => new Bookings{  StartDate=i.StartDate, EndDate=i.EndDate, RoomNumber=i.RoomNumber }).ToList(); 
            {

                foreach (var t in b) {
                    if (DateTime.Now <= t.EndDate && DateTime.Now >= t.StartDate)
                    {
                        SetAvailability(false, t.RoomNumber);
                    }
                } }

            GridView1.ItemsSource = rooms; */
        }

        public void SetAvailability(bool available, string RoomNumber)
        {
            string setAvailableQuery = "Update Rooms set Available=@Available where RoomNumber=@RoomNumber;";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MessageBox.DisplayDialog("Fehler",eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = setAvailableQuery;
                        cmd.Parameters.AddWithValue("@RoomNumber", RoomNumber);
                        cmd.Parameters.AddWithValue("@Available", available);
                        try
                        {

                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException exsql)
                        {
                            MessageBox.DisplayDialog("Fehler",exsql.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }

                    }
                }
            }
        }
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private async Task<List<Rooms>> GetRooms()
        {
            string getUserQuery = "Select * from Rooms;";
            var rooms = new List<Rooms>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MessageBox.DisplayDialog("Fehler", eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = getUserQuery;
                        SqlDataReader result = cmd.ExecuteReader();

                        while(result.Read())
                            {
                                var room = new Rooms
                                {
                                    ID = result.GetInt32(0),
                                    RoomNumber = result.GetString(1),
                                    NumberOfBeds = result.GetInt32(2),

                                    PricePerNight = Math.Round(result.GetDecimal(3), 2),
                                    RoomSize = result.GetString(4),
                                    ImageUrl = result.GetString(5),
                                    Available = result.GetBoolean(6),
                                    
                                    SourceImage = (byte[])result[7],
                                    
                                    
                                };
     
                                
                                rooms.Add(room);
                            foreach (var item in rooms)
                            {
                                room.Image = await BitmapImageConverter.ConvertByteToImage(room.SourceImage);
                            }
                        }
                            return rooms;

                        

                    }

                }


            }




            return null;
        }
        private void BtnSearch_Click(object sender,RoutedEventArgs args)
        {
            
         
                ComboBoxItem cbi = (ComboBoxItem)cboxSearch.SelectedItem;
                switch (cbi.Content.ToString())
                {
                    case "Zimmernummer":
                        {
                            r = rooms.Where(t => t.RoomNumber.Contains(asbSearch.Text)).ToList();
                            break;
                        }
                    case "Anzahl Betten":
                        {
                            r = rooms.Where(t => t.NumberOfBeds.ToString().Contains(asbSearch.Text)).ToList();
                            break;
                        }
                    case "Zimmergröße":
                        {
                            r = rooms.Where(t => t.RoomSize.Contains(asbSearch.Text)).ToList();
                            break;
                        }
                    case "Preis":
                        {
                        r = rooms.Where(t => t.PricePerNight.ToString().Contains(asbSearch.Text)).ToList();
                        break;
                        }
                case "Buchbar":
                    {
                        r = rooms.Where(t => t.Available == true).ToList();
                        break;
                    }
                case "Alle":
                    {
                        r = rooms;
                        break;
                    }
                default:
                    {
                        r = rooms;
                        break;
                    }
                }
           
                GridView1.ItemsSource = r;
            
            }
 
   
        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs args)
        {
            ((Window.Current.Content as Frame).Content as MainPage).contentFrame.Navigate(typeof(ReservedPage), rooms[GridView1.SelectedIndex].RoomNumber);
            
        }

        private List<Booking> GetBookings(string roomnumber)
        {
            string getUserQuery = "Select Booking.ID, Booking.CustomerID, Booking.RoomID, StartDate, EndDate, Lastname, Firstname, RoomNumber," +
                "Available from Booking inner join Customer on Booking.CustomerID=Customer.CustomerID inner join Rooms on Booking.RoomID = Rooms.ID where Rooms.RoomNumber=@RoomNumber;";
            var bookings = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MessageBox.DisplayDialog("Fehler",eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = getUserQuery;
                        cmd.Parameters.AddWithValue("@RoomNumber", roomnumber);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var booking = new Booking
                                {
                                    BookingID = reader.GetInt32(0),

                                    CustomerID = reader.GetInt32(1),
                                    RoomID = reader.GetInt32(2),

                                    StartDate = reader.GetDateTime(3),
                                    EndDate = reader.GetDateTime(4),
                                    Lastname = reader.GetString(5),
                                    Firstname = reader.GetString(6),
                                    RoomNumber = reader.GetString(7),
                                    Available = reader.GetBoolean(8),


                                };

                                bookings.Add(booking);


                            }

                            return bookings;

                        }

                    }

                }


            }




            return null;
        }
        public FrameworkElement FindVisualChildByName(DependencyObject obj, string name)
        {
            FrameworkElement ret = null;
            int numChildren = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < numChildren; i++)
            {
                var objChild = VisualTreeHelper.GetChild(obj, i);
                var child = objChild as FrameworkElement;
                if (child != null && child.Name == name)
                {
                    return child;
                }

                ret = FindVisualChildByName(objChild, name);
                if (ret != null)
                    break;
            }
            return ret;
        }

    }
}
