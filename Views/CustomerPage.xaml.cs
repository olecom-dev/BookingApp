using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
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
    public sealed partial class CustomerPage : Page
    {
 
        List<Countries> country = new List<Countries>();
        
        List<Customer> cus = new List<Customer>();
        public string ConnectionString = (App.Current as App).ConnectionString;
        ComboBox CmbSort = new ComboBox();

        public CustomerPage()
        {
            this.InitializeComponent();
            cboxBookings.Items.Add("Alle Kunden");
            cboxBookings.Items.Add("Aktuelle Buchungen");
            cboxBookings.Items.Add("Reservierungen");
            cboxBookings.Items.Add("Vergangene Buchungen");
            cboxBookings.SelectedIndex = 0;
            cus = GetCustomers(ConnectionString, cboxBookings.SelectedIndex);
            cus = new List<Customer>(cus.OrderBy(i => i.Lastname));
            country = GetCountries(ConnectionString);
            
            this.TboxCustomerID.Text = cus[0].CustomerID.ToString();
            this.TboxCustomerCode.Text = cus[0].CustomerCode;
            this.TboxLastname.Text = cus[0].Lastname;
            this.TboxFirstname.Text = cus[0].Firstname;
            this.TboxAddress.Text = cus[0].Address;
            this.TboxCity.Text = cus[0].City;
            this.TboxPostalcode.Text = cus[0].Postalcode;
            this.TboxEMailAdress.Text = cus[0].EMailAddress;
            this.CboxCountry.ItemsSource = country;
            this.CboxCountry.DisplayMemberPath = "Countryname";
            this.CboxCountry.SelectedValuePath = "Countrycode";
            this.CboxCountry.SelectedValue = cus[0].Countrycode;
            this.CDPDateOfBirth.Date = cus[0].DateOfBirth;
            this.TboxPhone.Text = cus[0].Phone;
            this.TboxMobilePhone.Text = cus[0].MobilePhone;
            this.TboxPassport.Text = cus[0].Passport;
            
            this.CustomerList.ItemsSource = cus;
            
           












        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string customerID = Environment.GetEnvironmentVariable("CustomerID");
            if (customerID == "-1")
            {
                ControlsEnabled(true);
                TboxCustomerID.Text = customerID;
                ControlsEmpty();

            }
            else
            {
                //  ControlsEnabled(false);
                TboxCustomerCode.IsEnabled = false;
                TboxCustomerID.IsEnabled = false;
                TboxLastname.IsEnabled = false;
                TboxFirstname.IsEnabled = false;
              TboxAddress.IsEnabled = false;
                TboxCity.IsEnabled = false;
                TboxEMailAdress.IsEnabled = false;
                TboxMobilePhone.IsEnabled = false;
                TboxPhone.IsEnabled = false;
                TboxPostalcode.IsEnabled = false;
                TboxPassport.IsEnabled = false;
                CboxCountry.IsEnabled = false;
                CDPDateOfBirth.IsEnabled = false;
                
                txtAutoComplete.IsEnabled = true;
            }
            IEnumerable<ComboBox> CmbSortList = FindVisualChildren<ComboBox>(CustomerList).Where(x => x.Name == "CmbSort");
            foreach (ComboBox cmbBox in CmbSortList)
            {
                if (cmbBox.Name == "CmbSort")
                {
                    CmbSort = cmbBox;
                }
                //   CmbSort = (ComboBox)CustomerList.FindName("CmbSort");
                //  CmbSort = (ComboBox)_Children.Find(c => c.Name == _Name);
             
            }
        }
        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)CustomerList.SelectedItem;
            

            string selectedCar = (string)cmb.SelectedItem;
            MessageBox.DisplayDialog("OK", selectedCar);
            
        }


        public void BtnCode_Click(object sender, RoutedEventArgs e)
        {
            if(CmbSort.SelectedIndex==0)
           
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.CustomerCode);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.CustomerCode);
        }
        public void BtnLastname_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.Lastname);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.Lastname);

        }
        public void BtnFirstname_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.Firstname);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.Firstname);

        }
        public void BtnAdress_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.Address);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.Address);

        }
        public void BtnCity_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.City);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.City);

        }
        public void BtnPostalcode_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.Postalcode);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.Postalcode);

        }
        private void BtnStartDate_Click(object sender, RoutedEventArgs e)
        {
            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.StartDate);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.StartDate);
        }
        private void BtnEndDate_Click(object sender, RoutedEventArgs e)
        {
            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderBy(i => i.EndDate);
            else
                this.CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex).OrderByDescending(i => i.EndDate);
        }
        void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemIndex % 2 != 0)
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                
            }
            else
            {
                args.ItemContainer.Background = new SolidColorBrush(Colors.LightSalmon);
            }
        }
        public List<Customer> GetCustomers(string connectionString, int index ) {
            string GetCustomersQuery = "";
            if (index == 0)
            {
                GetCustomersQuery = "Select * From Customer";
            }
            else if (index==1)
            {
                GetCustomersQuery = "Select Customer.*, Booking.ID, Booking.StartDate, Booking.EndDate from dbo.Customer  inner join Booking on Customer.CustomerID=Booking.CustomerID Where Booking.StartDate<@StartDate And Booking.EndDate>@EndDate";
            }
            else if (index==2)
            {
                GetCustomersQuery = "Select Customer.*, Booking.ID, Booking.StartDate, Booking.EndDate from dbo.Customer  inner join Booking on Customer.CustomerID=Booking.CustomerID where Booking.StartDate>@StartDate And Booking.EndDate>@EndDate";
            }
            else if (index == 3)
            {
                GetCustomersQuery = "Select Customer.*, Booking.ID, Booking.StartDate, Booking.EndDate from dbo.Customer  inner join Booking on Customer.CustomerID=Booking.CustomerID where Booking.EndDate<@EndDate and Booking.StartDate<@StartDate";
            }
            var customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception eSql)
                {
                    MessageBox.DisplayDialog("Fehler",eSql.Message);

                }
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = GetCustomersQuery;
                        if (index > 0)
                        {
                            cmd.Parameters.AddWithValue("@StartDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@EndDate", DateTime.Now);
                        }
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customer = new Customer
                                {
                                    CustomerID = reader.GetInt32(0),
                                    Lastname = reader.GetString(1),
                                    Firstname = reader.GetString(2),
                                    Address = reader.GetString(3),
                                    City = reader.GetString(4),
                                    Postalcode = reader.GetString(5),
                                    Countrycode = reader.GetString(6)
                                };
                                if (!reader.IsDBNull(7))
                                    customer.EMailAddress = reader.GetString(7);
                                if (!reader.IsDBNull(8))
                                    customer.DateOfBirth = reader.GetDateTime(8);
                                if (!reader.IsDBNull(9))
                                    customer.Phone = reader.GetString(9);
                                if (!reader.IsDBNull(10))
                                    customer.MobilePhone = reader.GetString(10);
                                if (!reader.IsDBNull(11))
                                    customer.Passport = reader.GetString(11);
                                if (!reader.IsDBNull(12))
                                    customer.CustomerCode = reader.GetString(12);
                                if (cboxBookings.SelectedIndex > 0)
                                    {
                                       
                                        if (!reader.IsDBNull(13))
                                            customer.BookingID = reader.GetInt32(13);
                                        if (!reader.IsDBNull(14))
                                            customer.StartDate = reader.GetDateTime(14);
                                        if (!reader.IsDBNull(15))
                                            customer.EndDate = reader.GetDateTime(15);
                                    }
                                customers.Add(customer);
                            }
                            return customers;

                        }
                    }

                }
                return null;
            }


        }
        public List<Countries> GetCountries(string connectionString)
        {
            const string getCountriesQuery = "Select * from Countries";
            var countries = new List<Countries>();
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                        cmd.CommandText = getCountriesQuery;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var country = new Countries
                                {
                                    Countrycode = reader.GetString(0),
                                    Countryname = reader.GetString(1),
                                    CountryNameInternational = reader.GetString(2)
                                };
                                countries.Add(country);
                            }
                            return countries;

                        }
                    }

                }


            }




            return null;

        }
        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            
            if (CustomerList.SelectedIndex > -1) {
                int customerID = (int)CustomerList.SelectedValue;
                this.TboxCustomerID.Text = customerID.ToString();
                this.TboxCustomerCode.Text = cus.Where(i => i.CustomerID == customerID).First().CustomerCode;
                this.TboxLastname.Text = cus.Where(i => i.CustomerID == customerID).First().Lastname;                                   
                this.TboxFirstname.Text = cus.Where(i => i.CustomerID == customerID).First().Firstname;
                this.TboxAddress.Text = cus.Where(i => i.CustomerID == customerID).First().Address;
                this.TboxCity.Text = cus.Where(i => i.CustomerID == customerID).First().City;
                this.TboxPostalcode.Text = cus.Where(i => i.CustomerID == customerID).First().Postalcode;
                this.TboxEMailAdress.Text = cus.Where(i => i.CustomerID == customerID).First().EMailAddress;
                this.CboxCountry.SelectedValue = cus.Where(i => i.CustomerID == customerID).First().Countrycode;
                this.CDPDateOfBirth.Date = cus.Where(i => i.CustomerID == customerID).First().DateOfBirth;
                this.TboxPassport.Text = cus.Where(i => i.CustomerID == customerID).First().Passport;
                if (cus.Where(i => i.CustomerID == customerID).First().Phone != null)
                    this.TboxPhone.Text = cus.Where(i => i.CustomerID == customerID).First().Phone;
                else
                    this.TboxPhone.Text = "";
                if (cus.Where(i => i.CustomerID == customerID).First().MobilePhone != null)
                    this.TboxMobilePhone.Text = cus.Where(i => i.CustomerID == customerID).First().MobilePhone;
                else
                    this.TboxMobilePhone.Text = ""; 
                        }
        }
        List<TextBox> AllTextBoxes(DependencyObject parent)
        {
            var list = new List<TextBox>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox)
                    list.Add(child as TextBox);
                list.AddRange(AllTextBoxes(child));
            }
            return list;
        }
        private void ThisList_ItemClick(object sender, RoutedEventArgs e)
        {

        }

        private void ControlsEnabled(bool isEnabled)
        {
            foreach (var textBox in AllTextBoxes(CustomerPanel))
            {
                textBox.IsEnabled = isEnabled;
                if(textBox.Name == "txtAutoComplete")
                {
                    textBox.IsEnabled = true;
                }
            }
            CboxCountry.IsEnabled = isEnabled;
           
            CDPDateOfBirth.IsEnabled = isEnabled;
        }
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            ControlsEnabled(true);
            ControlsEmpty();
            TboxCustomerCode.IsEnabled = false;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string customerCode = "K" + Regex.Replace(CDPDateOfBirth.SelectedDate.ToString(), @"\w", String.Empty) + TboxCustomerID.Text;
            Int32 lastInsertedID = 0;

            string insertCustomer = "Insert into Customer (Lastname, Firstname, Address, City, Postalcode, Countrycode, E_Mail, DateOfBirth, Phone, MobilePhone, Passport, CustomerCode) Output Inserted.CustomerID"+
                                            "values (@Lastname, @Firstname, @Address, @City, @Postalcode, @Countrycode, @E_Mail, @DateOfBirth, @Phone, @MobilePhone, @Passport, @CustomerCode);";
                    using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
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
                        cmd.CommandText = insertCustomer;
                        cmd.Parameters.AddWithValue("@Lastname", TboxLastname.Text);
                        cmd.Parameters.AddWithValue("@Firstname", TboxFirstname.Text);
                        cmd.Parameters.AddWithValue("@Address", TboxAddress.Text);
                        cmd.Parameters.AddWithValue("@City", TboxCity.Text);
                        cmd.Parameters.AddWithValue("@Postalcode", TboxPostalcode.Text);
                        cmd.Parameters.AddWithValue("@Countrycode", CboxCountry.SelectedValue);
                        cmd.Parameters.AddWithValue("@E_Mail", TboxEMailAdress.Text);
                        cmd.Parameters.AddWithValue("@DateOfBirth", CDPDateOfBirth.SelectedDate);
                        cmd.Parameters.AddWithValue("@Phone", TboxPhone.Text);
                        cmd.Parameters.AddWithValue("@MobilePhone", TboxMobilePhone.Text);
                        cmd.Parameters.AddWithValue("@Passport", TboxPassport.Text);
                        cmd.Parameters.AddWithValue("@CustomerCode", customerCode);

                        try
                        {
                            if (TboxLastname.Text != String.Empty && TboxFirstname.Text != String.Empty && TboxAddress.Text != String.Empty && TboxCity.Text != String.Empty)
                            {
                                lastInsertedID =  (Int32)cmd.ExecuteScalar();
                            }
                            else
                            {
                                MessageBox.DisplayDialog("Fehler", "Bitte füllen Sie alle Felder aus!");
                            }
                                }



                                catch (SqlException eSql)
                        {
                            MessageBox.DisplayDialog("Fehler", eSql.Message);
                        }
                    
                            }
                        }
                        conn.Close();
                    }

            Frame.Navigate(this.GetType());
        }
        private void CboxCustomers_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cboxCustomer.SelectedIndex >= 0)
            {
                CustomerList.ItemsSource = cus.Where(i => i.CustomerID == (int)cboxCustomer.SelectedValue);
            }
        }
        private void TxtAutoComplete_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent())
            {
                
                var search_term = txtAutoComplete.Text;
                var results = cus.Where(i => i.Lastname.StartsWith(search_term)).ToList();
                txtAutoComplete.ItemsSource = results;
                txtAutoComplete.DisplayMemberPath = "Lastname";
                Dictionary<int, string> comboSource = new Dictionary<int, string>();
                foreach (var item in results)
                {

                    string Fullname = (item.Lastname + ", " + item.Firstname);
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
            var results = cus.Where(i => i.Lastname.StartsWith(search_term)).ToList();
            txtAutoComplete.ItemsSource = results;
            txtAutoComplete.DisplayMemberPath = "Lastname";
            txtAutoComplete.IsSuggestionListOpen = true;
       //     CustomerList.ItemsSource = cus.Where(u => u.Lastname.StartsWith(txtAutoComplete.Text));
        }
        private void ControlsEmpty()
        {
            foreach (var textBox in AllTextBoxes(this))
            {
                textBox.Text=String.Empty;
            }
            TboxCustomerID.Text = "-1";
            CboxCountry.SelectedIndex = 0;
            CDPDateOfBirth.Date = DateTime.Now.Date;
        }
        public List<Booking> GetBookings(string connectionString)
        {
            const string getBookingsQuery = "Select * from Booking";
            var bookings = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                        cmd.CommandText = getBookingsQuery;
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
                                    PriceOverAll = reader.GetDecimal(5),
                                    PriceOverAllWithoutTax = reader.GetDecimal(6),
                                    Printed = reader.GetBoolean(7),
                                    Timestamp = reader.GetDateTime(8),
                             
                                        
                                    
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
        public   List<Rooms> GetRooms(string connectionString)
        {
            const string getRoomsQuery = "Select * from Rooms";
            var rooms = new List<Rooms>();
            using (SqlConnection conn = new SqlConnection(connectionString))
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
                        cmd.CommandText = getRoomsQuery;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var room = new Rooms
                                {
                                    ID = reader.GetInt32(0),
                                    RoomNumber = reader.GetString(1),
                                    NumberOfBeds = reader.GetInt32(2),
                                    PricePerNight = reader.GetDecimal(3),
                                    RoomSize = reader.GetString(4),
                                    ImageUrl = reader.GetString(5),
                                    Available = reader.GetBoolean(6),
                                    SourceImage = (byte[])reader[7],
                                    Description = reader.GetString(8),
          //                         Image = await BitmapImageConverter.ConvertByteToImage((byte[])reader[7])

                            };
                                rooms.Add(room);

                           
                            }
                            return rooms;

                        }
                    }

                }


            }




            return null;

        }
        private  void CustomerList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (cboxBookings.SelectedIndex > 0)
            {
                Customer cust = (Customer)(e.OriginalSource as FrameworkElement).DataContext;
                if (cust != null)
                {

                    int bookingID = cust.BookingID;
                    //   List<Rooms> rooms = GetRooms(ConnectionString).Result;
                    Environment.SetEnvironmentVariable("bookingID", bookingID.ToString());

                    Booking allCustomerBookings = (Booking)(from c in cus
                                                            join co in GetCountries(ConnectionString) on c.Countrycode equals co.Countrycode
                                                            join b in GetBookings(ConnectionString) on c.CustomerID equals b.CustomerID
                                                            join r in GetRooms(ConnectionString) on b.RoomID equals r.ID
                                                            select new Booking
                                                            {
                                                                CustomerID = c.CustomerID,
                                                                BookingID = b.BookingID,
                                                                RoomID = r.ID,
                                                                Firstname = c.Firstname,
                                                                Lastname = c.Lastname,
                                                                Address = c.Address,
                                                                City = c.City,
                                                                PostalCode = c.Postalcode,
                                                                CountryNameInternational = co.CountryNameInternational,
                                                                StartDate = b.StartDate,
                                                                EndDate = b.EndDate,
                                                                PricePerNight = r.PricePerNight,
                                                                PriceOverAll = b.PriceOverAll,
                                                                PriceOverAllWithoutTax = b.PriceOverAllWithoutTax,
                                                                Printed = b.Printed,
                                                                RoomNumber = r.RoomNumber,
                                                                Image = r.Image,
                                                                CustomerCode = c.CustomerCode,
                                                                Timestamp = b.Timestamp,
                                                                ImageUrl = r.ImageUrl,


                                                            }).ToList().Where(i => i.BookingID == bookingID).First();
                    Environment.SetEnvironmentVariable("printed", allCustomerBookings.Printed.ToString());
                    Frame.Navigate(typeof(PrintHotelBookingsPage), allCustomerBookings);
                }
            }
        }
        private void CboxBookings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CustomerList.ItemsSource = GetCustomers(ConnectionString, cboxBookings.SelectedIndex);
        }
    }
}
