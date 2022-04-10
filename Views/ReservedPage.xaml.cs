using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ReservedPage : Page
    {
        public string ConnectionString = (App.Current as App).ConnectionString;
        private string roomNumber;
        
        List<Customer> customerList = new List<Customer>();
        private List<DateTime> blackoutDates = new List<DateTime>();
   //     List<CustomerFullName> customerFullNames = new List<CustomerFullName>();
        private List<Booking> Bookings = new List<Booking>();
        private int countNights = 0;
        private int roomID;
        public Object taxRate;
        ApplicationDataContainer localSettings = null;

        public ReservedPage()
        {
            this.InitializeComponent();
            CalendarViewBookings.CalendarViewDayItemChanging += Calendar_CalendarViewDayItemChanging;
            CalendarViewBookings.SelectedDatesChanged += Calendar_CalendarViewSelectedDatesChanged;
            localSettings = ApplicationData.Current.LocalSettings;
          taxRate = localSettings.Values["TaxRate"];
            customerList = GetCustomers();

            //   dpReservedEnd.SelectedDate = DateTime.Now.Date;
            //   dpReservedStart.SelectedDate = DateTime.Now.Date;

            txtAutoComplete.ItemsSource = customerList;
            txtAutoComplete.DisplayMemberPath = "Lastname";
            



        }
    
        private void TxtAutoComplete_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                var search_term = txtAutoComplete.Text;
                var results = customerList.Where(i => i.Lastname.StartsWith(search_term)).ToList();
                txtAutoComplete.ItemsSource = results;
                txtAutoComplete.DisplayMemberPath = "Lastname";
                Dictionary<int, string> comboSource = new Dictionary<int, string>();
                foreach (var item in results)
                {

                    string Fullname = (item.DateOfBirth.ToString("dd MM yyyy") + "  " + item.Lastname + ", " + item.Firstname);
                    int ID = item.CustomerID;
                    comboSource.Add(ID, Fullname);


                }
                cboxCustomer.ItemsSource = comboSource.ToList();
                cboxCustomer.DisplayMemberPath = "Value";
                cboxCustomer.SelectedValuePath = "Key";
            }
        }

        private void TxtAutoComplete_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            txtAutoComplete.Text = ((Customer)args.SelectedItem).Lastname;

        }
        private void TxtAutoComplete_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var search_term = args.QueryText;
            var results = customerList.Where(i => i.Lastname.StartsWith(search_term)).ToList();
            txtAutoComplete.ItemsSource = results;
            txtAutoComplete.DisplayMemberPath = "Lastname";
            txtAutoComplete.IsSuggestionListOpen = true;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var roomNumber = e.Parameter;
            this.roomNumber = roomNumber.ToString();
            TblockTitle.Text = "Buchungen für Zimmer " + this.roomNumber;
            SetAvailability(true, this.roomNumber);
            Bookings = GetBookings(this.roomNumber);
         GetRoomID(this.roomNumber);
            dpReservedEnd.IsEnabled = false;
            




        }
        private List<Booking> GetBookings(string roomnumber)
        {
            string getUserQuery = "Select Booking.ID, Booking.CustomerID, Booking.RoomID, StartDate, EndDate, Lastname, Firstname, RoomNumber," +
                "Available, PricePerNight from Booking inner join Customer on Booking.CustomerID=Customer.CustomerID inner join Rooms on Booking.RoomID = Rooms.ID where Rooms.RoomNumber=@RoomNumber;";
            var bookings = new List<Booking>();
            
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MsgBox.Show(eSql.Message);
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
                                    PricePerNight = reader.GetDecimal(9)

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
        private List<Customer> GetCustomers()
        {
            string getCustomerQuery = "Select CustomerID, Lastname, Firstname, DateOfBirth From Customer";
            var customers = new List<Customer>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MsgBox.Show(eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = getCustomerQuery;
                        
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customer = new Customer
                                {
                                    CustomerID = reader.GetInt32(0),
                                    Lastname = reader.GetString(1),
                                    Firstname = reader.GetString(2),
                                    DateOfBirth = reader.GetDateTime(3)



                                };

                                customers.Add(customer);

                            }

                            return customers;

                        }

                    }

                }


            }




            return null;
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
        private void Calendar_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
            var textBlock = FindFirstChildOfType<TextBlock>(args.Item);
            if (textBlock != null)
            {
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Height = 20;
                textBlock.FontSize = 14;

                textBlock.FontWeight = FontWeights.Normal;

            }
            dpReservedEnd.Visibility = Visibility.Visible;
            if (args.Item.Date.Date.Equals(DateTime.Now.Date))
            {
                args.Item.Background = greenBrush;
            }

           
           // SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            List<string> listname = new List<string>();
            DateTime StartDate;
            DateTime EndDate;
            


            foreach (var item in GetBookings(roomNumber))
            {
                 StartDate = item.StartDate;
                 EndDate = item.EndDate;
                tbPricePerNight.Text = Math.Round(item.PricePerNight, 2).ToString();
                String Fullname = item.Lastname + ", " + item.Firstname;
                foreach (DateTime day in EachCalendarDay(StartDate, EndDate))
                {

                    List<Color> densityColors = new List<Color>
                    {
                        //  densityColors.Add(Colors.DarkRed);
                        Colors.DarkGreen
                    };


                    if (args.Item.Date.Date.Equals(DateTime.Now.Date)&&!DateTime.Now.Date.Equals(day))
                    {

                        args.Item.Background = greenBrush;



                    }
                    else if (args.Item.Date.Date.Equals(day))                                        
                    {


                        args.Item.Background = redBrush;
                        listname.Add(item.Lastname);
                        args.Item.IsBlackout = true;
                        blackoutDates.Add(args.Item.Date.Date);
                        SetAvailability(false, roomNumber);
                        dpReservedEnd.Visibility = Visibility.Collapsed;
                        
                        textBlock.Text = args.Item.Date.Day.ToString() + "  " + item.Lastname + ", " + item.Firstname;

                        
                        
                    }

                   
     

       




                    



                    



                }





            }
        }
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }
        private void Calendar_CalendarViewSelectedDatesChanged(object sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
        


        }
        private void CalendarView_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private void DpReservedStart_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {

            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.LightGreen);
            dpReservedStart.Background = greenBrush;
            if (dpReservedStart.Date!=null)
            {
                dpReservedEnd.IsEnabled = true;
                tbPriceOverall.Text = (Decimal.Parse(tbPricePerNight.Text) * countNights).ToString();
            }
            for (int i = 0; i < blackoutDates.Count(); i++)
            {

                if (e.NewDate.Date.Equals(blackoutDates[i].Date)||DateTime.Now.Equals(blackoutDates[i].Date))
                {
                    MsgBox.Show("Datum nicht verfügbar");
                    dpReservedStart.Background = redBrush;
                    
                }



            }


        }
        private void DpReservedEnd_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
           

            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.LightGreen);
            dpReservedEnd.Background = greenBrush;
            
            for (int i=0;i< blackoutDates.Count(); i++)
            {

                if (e.NewDate.Date.Equals(blackoutDates[i].Date))
                {
                    MsgBox.Show("Datum nicht verfügbar");
                    dpReservedEnd.Background = redBrush;
                    dpReservedEnd.SelectedDate = DateTime.Now;

                }

                
                    
                   
                

                
              
            }
            countNights = dpReservedEnd.Date.Day - dpReservedStart.Date.Day;
            tbPriceOverall.Text = (Math.Round(Decimal.Parse(tbPricePerNight.Text),2) * countNights).ToString();
        }

        private void CalendarViewBookings_SelectedDatesChanged(object sender, CalendarViewSelectedDatesChangedEventArgs e)
        {
            
          
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var priceWithoutTax = Decimal.Parse(tbPriceOverall.Text) - (Decimal.Parse(tbPriceOverall.Text) * Decimal.Parse(taxRate.ToString()) / 100);
            string insertBooking = "Insert into Booking (CustomerID, RoomID, StartDate, EndDate, PriceOverall, PriceOverAllWithoutTax) Values (@CustomerID, @RoomID, @StartDate, @EndDate, @PriceOverAll, @PriceOverAllWithoutTax);";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MsgBox.Show(eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = insertBooking;
                        cmd.Parameters.AddWithValue("@CustomerID", cboxCustomer.SelectedValue);
                        cmd.Parameters.AddWithValue("@RoomID", GetRoomID(this.roomNumber));
                        cmd.Parameters.AddWithValue("@StartDate", dpReservedStart.Date);
                        cmd.Parameters.AddWithValue("@EndDate", dpReservedEnd.Date);
                        cmd.Parameters.AddWithValue("@PriceOverAll", Decimal.Parse(tbPriceOverall.Text));
                        cmd.Parameters.AddWithValue("@PriceOverAllWithoutTax", priceWithoutTax);
                        try
                        {

                           int counter= cmd.ExecuteNonQuery();
                            if (counter > 0)
                            {
                                MsgBox.Show("Buchung erfolgreich durchgeführt");
                            }
                            else
                            {
                                MsgBox.Show("Fehler bei der Buchung");
                            }

                        }
                        catch (SqlException exsql)
                        {
                            MsgBox.Show(exsql.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }

                    }
                }
            }
            ((Window.Current.Content as Frame).Content as MainPage).contentFrame.Navigate(typeof(ReservedPage), this.roomNumber);
        }
        private int GetRoomID(string roomNumber)
        {
            
            string queryRoomID = "Select ID, PricePerNight from Rooms where RoomNumber=@RoomNumber;";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException eSql)
                {
                    MsgBox.Show(eSql.Message);
                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = queryRoomID;
                        cmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                        try
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {

                                    roomID = reader.GetInt32(0);
                                    var price = reader.GetDecimal(1);

                                    tbPricePerNight.Text = Math.Round(price, 2).ToString();






                                }
                            }

                           
                        }
                        catch (SqlException exsql)
                        {
                            MsgBox.Show(exsql.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                        return roomID;
                    }
                }
            }
            return -1;
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
                    MsgBox.Show(eSql.Message);
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
                            MsgBox.Show(exsql.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }

                    }
                }
            }
        }
    }
}