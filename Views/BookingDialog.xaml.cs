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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    public sealed partial class BookingDialog : ContentDialog
    {
      //  CalendarViewDayItem dayitem = new CalendarViewDayItem();
        public string ConnectionString = (App.Current as App).ConnectionString;
     //   List<Booking> bo = new List<Booking>();
        private readonly string RoomNumber;

        public BookingDialog(string RoomNumber)
        {
            this.InitializeComponent();
            Calendar.CalendarViewDayItemChanging += Calendar_CalendarViewDayItemChanging;


            this.RoomNumber = RoomNumber;
            this.Title = "Buchungen";
          //  bo = GetBookings();


        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ((Window.Current.Content as Frame).Content as MainPage).contentFrame.Navigate(typeof(ReservedPage), RoomNumber);
            this.Hide();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {


        }


        private void Calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {

            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
           SolidColorBrush blackBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            TboxName.Text = "";
            // Register callback for next phase.

            // Register callback for next phase.

            foreach (var item in GetBookings())
            {
                DateTime StartDate = item.StartDate;
                DateTime EndDate = item.EndDate;
                String Name = item.Lastname;


                CalendarViewDayItem dayitem = args.Item;






                this.Title = "Buchungen für Zimmer " + RoomNumber;
                foreach (DateTime day in EachCalendarDay(StartDate, EndDate))
                {

                    if (args.Item.Date.Date.Equals(DateTime.Now.Date))
                    {
                        dayitem.Background = greenBrush;



                    }
                    else if (args.Item.Date.Date.Equals(day))
                    {
                        
                      dayitem.Background = redBrush;
                      
                        




                    }
                    else if (!args.Item.Date.Equals(day))
                    {
                        
                    }



                }


            }
        }
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private List<Booking> GetBookings()
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
                        cmd.Parameters.AddWithValue("@RoomNumber", RoomNumber);
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
        private void Calendar_SelectedDatesChanged(object sender, CalendarViewSelectedDatesChangedEventArgs e)
        {
            try
            {
                var d = e.AddedDates[0];
                CalendarViewDayItem it = new CalendarViewDayItem();
                if (d != null)
                {
                    var days1 = from b in GetBookings() where b.EndDate.AddDays(1) > d & b.StartDate < d select b;
                    if (days1.Count() > 0)
                    {
                        TboxName.Text = "Gebucht von: " + days1.ElementAt(0).Lastname +", " + days1.ElementAt(0).Firstname;
                    }


                    else
                    {
                        TboxName.Text = "Buchbar:";
                    }
                }
            }
            catch (Exception)
            {

            }

        }

    }
}
