using BookingApp.Classes;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
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
    public sealed partial class PrintHotelBookingsPage : Page
    {
        
        int bookingID = 0;
        string billNumber = "";
        private PrintHelper _printHelper;
        public string ConnectionString = (App.Current as App).ConnectionString;
        Booking book = new Booking();
       Style style = Application.Current.Resources["MyButtonStyle"] as Style;
        public PrintHotelBookingsPage()
        {
            this.InitializeComponent();
            
            
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bookingID = Int32.Parse(Environment.GetEnvironmentVariable("bookingID"));
            List<Booking> listBookings = new List<Booking>();
            Dictionary<int, BitmapImage> listViewSource = new Dictionary<int, BitmapImage>();
            bool isPrinted = Boolean.Parse(Environment.GetEnvironmentVariable("printed"));
            PrintBookingsDialog pbd = new PrintBookingsDialog(isPrinted);
            ContentDialogResult result = await pbd.ShowAsync();
            base.OnNavigatedTo(e);
            if (result == ContentDialogResult.Primary && pbd.PrimaryButtonText == "Anzeigen")
            {
                book = (Booking)e.Parameter;
            

            listBookings.Add(book);
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder images = await appInstalledFolder.GetFolderAsync("Images");
            StorageFile file = await images.GetFileAsync(book.ImageUrl);

            Windows.Storage.Streams.IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
            BitmapImage bitimg = new BitmapImage();
            bitimg.SetSource(stream);
            listViewSource.Add(0, bitimg);
            book.Image = bitimg;
            ListViewBookings.ItemsSource = listBookings;
            if (bookingID != 0 && book != null)
            {

                billNumber = "H" + Regex.Replace(book.Timestamp.ToString("dd-MM-yy"), @"\W", String.Empty) + (book.BookingID.ToString() + "0000").Substring(0, 3);
                //       MessageBox.DisplayDialog("Test",billNumber);


                PrintHotelBookings.Items.Add("Rechnungs-Nr.: " + billNumber);
                PrintHotelBookings.Items.Add("Kunden-Nr.: " + book.CustomerCode + Environment.NewLine + "Gebucht vom " + book.StartDate.ToShortDateString() + " bis zum " + book.EndDate.ToShortDateString());
                PrintHotelBookings.Items.Add("___________________________________________");
                PrintHotelBookings.Items.Add(book.Lastname + ", " + book.Firstname);
                PrintHotelBookings.Items.Add(book.Address);
                PrintHotelBookings.Items.Add(book.PostalCode.Trim() + " " + book.City.Trim());
                PrintHotelBookings.Items.Add(book.CountryNameInternational);
                PrintHotelBookings.Items.Add(Environment.NewLine);
                PrintHotelBookings.Items.Add("Zimmernummer:                                 " + book.RoomNumber);
                PrintHotelBookings.Items.Add("Preis pro Übernachtung:              " + book.PricePerNight.ToString("0.00") + " €");
                if (book.PricePerNight != 0)
                {
                    PrintHotelBookings.Items.Add("Übernachtungen:                                  " + (book.PriceOverAll / book.PricePerNight));
                }
                PrintHotelBookings.Items.Add(Environment.NewLine);
                PrintHotelBookings.Items.Add("Gesamtpreis:                                " + book.PriceOverAll.ToString("0.00") + " €");
                PrintHotelBookings.Items.Add("Gesamtpreis ohne MwSt:             " + book.PriceOverAllWithoutTax.ToString("0.00") + " €");
                PrintHotelBookings.Items.Add(Environment.NewLine);
                PrintHotelBookings.Items.Add("Vielen Dank für Ihren Besuch" + Environment.NewLine + "und kommen Sie bald wieder!");
                //   PrintHotelBookings.Items.Add(new Img("Value", img));
            }
        }
        else if(result== ContentDialogResult.Primary && pbd.PrimaryButtonText == "OK")
            {
                foreach (var item in GetBill(ConnectionString, 1))
                    book = JsonConvert.DeserializeObject<Booking>(item.Text);
                Frame.Navigate(typeof(PrintEverythingPage), book);
            }
            else if (result== ContentDialogResult.Secondary)
            {
                Frame.Navigate(typeof(CustomerPage));
            }
}
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
           
            _printHelper = new PrintHelper(Container);
            ListBox listBox = new ListBox();
                var grid = new Grid();
                //grid.Height = 600;
                RowDefinition rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rd);
            // ListBox listBox = new ListBox();
            listBox.Background = new SolidColorBrush(Colors.Transparent);


                //      MessageBox.DisplayDialog("Test", item.Countrycode);
                ListBoxItem lbi = new ListBoxItem();
                ListBoxItem lbi1 = new ListBoxItem();
            ListBoxItem lbi0 = new ListBoxItem();

                ListBoxItem lbi2 = new ListBoxItem();
            lbi.Content = "Rechnungs-Nr.: " + billNumber;  
                lbi0.Content="Kunden-Nr.: "+ book.CustomerCode +  Environment.NewLine +"Gebucht vom "+ book.StartDate.ToShortDateString() + " bis zum "+ book.EndDate.ToShortDateString();
                lbi0.Margin = new Thickness(0);
                lbi0.Padding = new Thickness(0);
                
                lbi.Margin = new Thickness(0);
                lbi.Padding = new Thickness(0);
                listBox.Items.Add(lbi);
            listBox.Items.Add(lbi0);
            lbi1.Margin = new Thickness(0);
                lbi1.Padding = new Thickness(0);
                lbi1.Content = "___________________________________________";
                listBox.Items.Add(lbi1);
                lbi2.Margin = new Thickness(0);
                lbi2.Padding = new Thickness(0);
                lbi2.Content = book.Lastname + ", " + book.Firstname;
                listBox.Items.Add(lbi2);
                ListBoxItem lbi3 = new ListBoxItem();
                lbi3.Margin = new Thickness(0);
                lbi3.Padding = new Thickness(0);
                lbi3.Content = book.Address;
                listBox.Items.Add(lbi3);
                ListBoxItem lbi4 = new ListBoxItem();
                lbi4.Margin = new Thickness(0);
                lbi4.Padding = new Thickness(0);
                lbi4.Content = book.PostalCode.Trim() + " " + book.City.Trim();
                listBox.Items.Add(lbi4);
                ListBoxItem lbi5 = new ListBoxItem();
                lbi5.Margin = new Thickness(0);
                lbi5.Padding = new Thickness(0);
                lbi5.Content = book.CountryNameInternational;
                listBox.Items.Add(lbi5);
                listBox.Items.Add(Environment.NewLine);
                ListBoxItem lbi6 = new ListBoxItem();
                lbi6.Margin = new Thickness(0);
                lbi6.Padding = new Thickness(0);
                lbi6.Content = "Zimmernummer:                                 " + book.RoomNumber;
                listBox.Items.Add(lbi6);
                ListBoxItem lbi7 = new ListBoxItem();
                lbi7.Margin = new Thickness(0);
                lbi7.Padding = new Thickness(0);
                lbi7.Content = "Preis pro Übernachtung:              " + book.PricePerNight.ToString("0.00") + " €";
                listBox.Items.Add(lbi7);
                ListBoxItem lbi11 = new ListBoxItem();
                lbi11.Margin = new Thickness(0);
                lbi11.Padding = new Thickness(0);
                if (book.PricePerNight != 0)
                {
                    lbi11.Content = "Übernachtungen:\t\t\t" + (book.PriceOverAll / book.PricePerNight);
                    listBox.Items.Add(lbi11);
                }
                ListBoxItem lbi8 = new ListBoxItem();
                lbi8.Margin = new Thickness(0);
                lbi8.Padding = new Thickness(0);
                lbi8.Content = "Gesamtpreis:                                " + book.PriceOverAll.ToString("0.00") + " €";
               listBox.Items.Add(Environment.NewLine);
                listBox.Items.Add(lbi8);
                ListBoxItem lbi9 = new ListBoxItem();
                lbi9.Margin = new Thickness(0);
                lbi9.Padding = new Thickness(0);
                lbi9.Content = "Gesamtpreis ohne MwSt:             " + book.PriceOverAllWithoutTax.ToString("0.00") + " €";
                listBox.Items.Add(lbi9);
                ListBoxItem lbi10 = new ListBoxItem();
                lbi10.Margin = new Thickness(0);
                lbi10.Padding = new Thickness(0);
                lbi10.Content = "Vielen Dank für Ihren Besuch" + Environment.NewLine + "und kommen Sie bald wieder!";
                listBox.Items.Add(Environment.NewLine);
                listBox.Items.Add(lbi10);
      //          listBox.Style = style;
              
            




            try
                {

                    grid.Children.Add(listBox);




                }
                catch (Exception ex)
                {
                    MessageBox.DisplayDialog("Fehler", ex.Message);
                }
                _printHelper.AddFrameworkElementToPrint(grid);

            
            var printHelperOptions = new PrintHelperOptions(false);
            printHelperOptions.Orientation = Windows.Graphics.Printing.PrintOrientation.Default;
            printHelperOptions.AddDisplayOption(StandardPrintTaskOptions.Orientation);
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
            await _printHelper.ShowPrintUIAsync("Rechnung drucken", printHelperOptions);

        }
        private async void PrintHelper_OnPrintFailed()
        {
            _printHelper.Dispose();
            var dialog = new MessageDialog("Rechnung konnte nicht gedruckt werden.");
            await dialog.ShowAsync();
        }
        /// <summary>
        ///
        /// </summary>
        private async void PrintHelper_OnPrintSucceeded()
        {
            _printHelper.Dispose();

            var dialog = new MessageDialog("Rechnung gedruckt.");
            await dialog.ShowAsync();

            string json = JsonConvert.SerializeObject(book);

            int billID = 0;
            string insertBills = "Insert into Bills (BillNumber, Text, Timestamp) OUTPUT INSERTED.ID Values (@BillNumber, @Text, @Timestamp)";
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
                        cmd.CommandText = insertBills;

                        cmd.Parameters.AddWithValue("@BillNumber", billNumber);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Text", json);


                        try
                        {
                        billID = (int)cmd.ExecuteScalar();

                        }
                        catch (SqlException eSql)
                        {
                            MessageBox.DisplayDialog("Fehler", eSql.Message);
                        }
                    }
                }
                conn.Close();

            }
 //           string test = "UPDATE Booking SET Booking.BillID = (SELECT Bills.ID FROM Bills WHERE Bills.Timestamp = Persons.PersonId)";

        //    MessageBox.DisplayDialog("Test", billID.ToString());

        string updateAllBookings = "Update Booking set Printed=@Printed, BillID=@BillID Where ID=@BookingID";
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
                        cmd.CommandText = updateAllBookings;

                        cmd.Parameters.AddWithValue("@Printed", true);
                        cmd.Parameters.AddWithValue("@BillID", billID);
                        cmd.Parameters.AddWithValue("@BookingID",book.BookingID);


                        try
                        {
                            cmd.ExecuteNonQuery();

                        }
                        catch (SqlException eSql)
                        {
                            MessageBox.DisplayDialog("Fehler", eSql.Message);
                        }
                    }
                }
                conn.Close();

            }


            Frame.Navigate(typeof(CustomerPage));
        }
        private List<Bills> GetBill(string connectionString, int billID)
        {
            List<Bills> bills = new List<Bills>();
            string updateAllBookings = "Select * from Bills Where ID=@BillID";
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
                        cmd.CommandText = updateAllBookings;

                        cmd.Parameters.AddWithValue("@BillID", billID);
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Bills bill = new Bills
                            {
                                BillID = reader.GetInt32(0),
                                BillNumber = reader.GetString(1),
                                Text = reader.GetString(2),
                                Timestamp = reader.GetDateTime(3)














                            };
                            bills.Add(bill);

                        }
                        return bills;
                    }

                }
                conn.Close();

            }
            return null;
        } 
    }
    
 
}
