using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class AllBookingsPage : Page
    {
        public string ConnectionString = (App.Current as App).ConnectionString;
        public List<Rooms> numbers = new List<Rooms>();
        public AllBookingsPage()
        {
            this.InitializeComponent();
            Style style = App.Current.Resources["MyCalendarViewItemStyle"] as Style;
            foreach (var i in GetRoomID())
            {

                numbers.Add(i);
                string xamlCV = "<CalendarView Name='" + i.RoomNumber.ToString() + "' xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Margin='10'  Width='1500' CalendarViewDayItemStyle='{StaticResource MyCalendarViewItemStyle}' >" +
                    "</CalendarView>";
                string xamlTB = "<Button  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Margin='10, 0,0,0' ></Button>";
                Button t = (Button)XamlReader.Load(xamlTB);
                CalendarView control = (CalendarView)XamlReader.Load(xamlCV);
                control.CalendarViewDayItemChanging += (s, e) => Calendar_CalendarViewDayItemChanging(s, e, i.ID);
                t.Click += (s, e) => Button_Click(s, e, i.RoomNumber);
             //  control.Style = this.Resources["MyCalendar"] as Style;
              //  control.CalendarViewDayItemStyle = style;
                t.Content = "Buchungen für Zimmer " + i.RoomNumber.ToString();
                t.FontSize = 16;
                MyStackPanel.Children.Add(t as UIElement);
                MyStackPanel.Children.Add(control as UIElement);

            }
        }
        private List<Rooms> GetRoomID()
        {
            Rooms i = new Rooms();
            List<Rooms> roomNumbers = new List<Rooms>();
            string queryRoomID = "Select  * from Rooms;";
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
                        cmd.CommandText = queryRoomID;

                        try
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Rooms r = new Rooms();
                                    r.ID = reader.GetInt32(0);
                                    r.RoomNumber = reader.GetString(1);



                                    roomNumbers.Add(r);




                                }
                                return roomNumbers;
                            }


                        }
                        catch (SqlException exsql)
                        {
                            MessageBox.DisplayDialog("Fehler",exsql.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                        return roomNumbers;
                    }

                }
            }
            return null;
        }
        private void Calendar_CalendarViewDayItemChanging(object sender, CalendarViewDayItemChangingEventArgs args, int RoomID)
        {

            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
            SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);

            var textBlock = FindFirstChildOfType<TextBlock>(args.Item);
            if (textBlock != null)
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Height = 20;
                textBlock.FontSize = 14;
                
                textBlock.FontWeight = FontWeights.Normal;
                
            } 
    
            List<String> listname = new List<string>();
            DateTime StartDate;
            DateTime EndDate;
            if (args.Item.Date.Date.Equals(DateTime.Now.Date))
            {

                args.Item.Background = greenBrush;



            }




            foreach (var item in GetBookings(RoomID))
            {

                StartDate = item.StartDate;
                EndDate = item.EndDate;

                String Fullname = item.Lastname + ", " + item.Firstname;
                foreach (DateTime day in EachCalendarDay(StartDate, EndDate))
                {

                    List<Color> densityColors = new List<Color>();
                    //  densityColors.Add(Colors.DarkRed);
                    densityColors.Add(Colors.DarkGreen);


                    if (args.Item.Date.Date.Equals(DateTime.Now.Date)&& args.Item.Date.Date.Equals(day))
                    {

                        args.Item.Background = greenBrush;
                        textBlock.Text = args.Item.Date.Day + "  " + Fullname;


                    }
                    else if (args.Item.Date.Date.Equals(day) || DateTime.Now.Equals(day))
                    {


                        args.Item.Background = redBrush;
                        textBlock.Foreground = whiteBrush;
                        args.Item.VerticalAlignment = VerticalAlignment.Center;
                        listname.Add(Fullname);
                        textBlock.Text = args.Item.Date.Day +"  "+Fullname;
                        args.Item.IsBlackout = true;




                    }











                    




                }


                



            }
          
        }
        private List<Booking> GetBookings(int id)
        {
            string getUserQuery = "Select Booking.ID, Booking.CustomerID, Booking.RoomID, StartDate, EndDate, Lastname, Firstname, RoomNumber," +
                "Available from Booking inner join Customer on Booking.CustomerID=Customer.CustomerID inner join Rooms on Booking.RoomID = Rooms.ID where Rooms.Id=@id;";
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
                        cmd.Parameters.AddWithValue("@id", id);
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
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private void Button_Click(object sender, RoutedEventArgs e, string RoomNumber)
        {
            ((Window.Current.Content as Frame).Content as MainPage).contentFrame.Navigate(typeof(ReservedPage), RoomNumber);
        }
        private T FindFirstChildOfType<T>(DependencyObject control) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(control);
            for (int childIndex = 0; childIndex < childrenCount; childIndex++)
            {
                var child = VisualTreeHelper.GetChild(control, childIndex);
                if (child is T typedChild)
                {
                    return typedChild;
                }
            }
            return null;
        }
    }

}
