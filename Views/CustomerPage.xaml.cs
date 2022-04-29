using BookingApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CustomerPage : Page
    {
 
        List<Countries> country = new List<Countries>();
        
        ObservableCollection<Customer> cus = new ObservableCollection<Customer>();
        public string ConnectionString = (App.Current as App).ConnectionString;
        

        public CustomerPage()
        {
            this.InitializeComponent();
            
            cus = GetCustomers(ConnectionString);
            cus = new ObservableCollection<Customer>(cus.OrderBy(i => i.Lastname));
            country = GetCountries(ConnectionString);
            
            this.TboxCustomerID.Text = cus[0].CustomerID.ToString();
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
            ControlsEnabled(false);



            
           
            {

            }

            
 
        


        }
        public void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)CustomerList.SelectedItem;
            

            string selectedCar = (string)cmb.SelectedItem;
            MessageBox.DisplayDialog("OK", selectedCar);
            
        }

        public void BtnID_Click(object sender, RoutedEventArgs e)
        {
            if(CmbSort.SelectedIndex==0)
           
                this.CustomerList.ItemsSource = cus.OrderBy(i => i.CustomerID);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.CustomerID);
        }
        public void BtnLastname_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = cus.OrderBy(i => i.Lastname);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.Lastname);

        }
        public void BtnFirstname_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = cus.OrderBy(i => i.Firstname);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.Firstname);

        }
        public void BtnAdress_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = cus.OrderBy(i => i.Address);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.Address);

        }
        public void BtnCity_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = cus.OrderBy(i => i.City);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.City);

        }
        public void BtnPostalcode_Click(object sender, RoutedEventArgs e)
        {

            if (CmbSort.SelectedIndex == 0)

                this.CustomerList.ItemsSource = cus.OrderBy(i => i.Postalcode);
            else
                this.CustomerList.ItemsSource = cus.OrderByDescending(i => i.Postalcode);

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
        public ObservableCollection<Customer> GetCustomers(string connectionString ) {

           
            
                const string GetCustomersQuery = "Select * from dbo.Customer";
            var customers = new ObservableCollection<Customer>();

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
            const string getCountriesQuery = "Select Countrycode, Countryname from Countries";
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
                                    Countryname = reader.GetString(1)
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
            int i = CustomerList.SelectedIndex;
            this.TboxCustomerID.Text = cus[i].CustomerID.ToString();
            this.TboxLastname.Text = cus[i].Lastname;
            this.TboxFirstname.Text = cus[i].Firstname;
            this.TboxAddress.Text = cus[i].Address;
            this.TboxCity.Text = cus[i].City;
            this.TboxPostalcode.Text = cus[i].Postalcode;
            this.TboxEMailAdress.Text = cus[i].EMailAddress;
            this.CboxCountry.SelectedValue = cus[i].Countrycode;
            this.CDPDateOfBirth.Date = cus[i].DateOfBirth;
            this.TboxPassport.Text = cus[i].Passport;
            if (cus[i].Phone != null)
                this.TboxPhone.Text = cus[i].Phone;
            else
                this.TboxPhone.Text = "";
            if (cus[i].MobilePhone != null)
                this.TboxMobilePhone.Text = cus[i].MobilePhone;
            else
                this.TboxMobilePhone.Text = "";
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
            foreach (var textBox in AllTextBoxes(this))
            {
                textBox.IsEnabled = isEnabled;
            }
            CboxCountry.IsEnabled = isEnabled;
            CDPDateOfBirth.IsEnabled = isEnabled;
        }
        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            ControlsEnabled(true);
            ControlsEmpty();
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            string insertCustomer = "Insert into Customer (Lastname, Firstname, Address, City, Postalcode, Countrycode, E_Mail, DateOfBirth, Phone, MobilePhone, Passport)" +
                                    "values (@Lastname, @Firstname, @Address, @City, @Postalcode, @Countrycode, @E_Mail, @DateOfBirth, @Phone, @MobilePhone, @Passport);";
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

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch(SqlException eSql)
                        {
                            MessageBox.DisplayDialog("Fehler", eSql.Message);
                        }
                        }
                }
                conn.Close();
            }
        }
        private void ControlsEmpty()
        {
            foreach (var textBox in AllTextBoxes(this))
            {
                textBox.Text=String.Empty;
            }
            TboxCustomerID.Text = "-1";
            CboxCountry.SelectedIndex = -1;
            CDPDateOfBirth.Date = DateTime.Now.Date;
        }

    }
}
