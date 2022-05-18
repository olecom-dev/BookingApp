using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using BookingApp.Classes;
using System.Data.SqlClient;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Printing;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Microsoft.Toolkit;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PrintAllRestaurantBookings : Page
    {
        List<AllRestaurantBookings> printList = new List<AllRestaurantBookings>();
        List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        List<RestaurantBooking> resBook = new List<RestaurantBooking>();
        List<Products> prods = new List<Products>();
        ListBoxItem i = new ListBoxItem();
        Decimal price = 0;
        string code = "";
        int multiplicator = 0;
        Decimal pricecomplete = 0;
        decimal priceAdded = 0;
        decimal p = 0;
        string desc = "";
        Decimal price1 = 0;
        string code1 = "";
        int multiplicator1 = 0;
        Decimal pricecomplete1 = 0;
        decimal priceAdded1 = 0;
        decimal p1 = 0;
        string desc1 = "";
        private PrintHelper _printHelper;


        List<BookingCodes> codes = new List<BookingCodes>();
        public PrintAllRestaurantBookings()
        {
            this.InitializeComponent();




            // resBook.Add(new RestaurantBooking { BookingCode = code, Multiplicator = multiplicator, Price = price });

        }
        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
       //     var bookingCode = "";
            List<Products> products = GetAllCodes((App.Current as App).ConnectionString);
            printList = GetAllRestaurantBookings((App.Current as App).ConnectionString);
            foreach (var item in printList)
            {
                restaurantBookings.AddRange(JsonConvert.DeserializeObject<List<RestaurantBooking>>(item.Text));
            }


      //       MessageBox.DisplayDialog("Test", restaurantBookings.Count.ToString());
            if (restaurantBookings.Count > 0)
            {
                ListBoxPrint.Items.Add(String.Format("Einnahmen von\t{0}\tbis\t{1}", printList.Select(i => i.Timestamp.ToString()).First(), DateTime.Now.ToString()));
                ListBoxPrint.Items.Add("===============================================================================================");
                foreach (var res in restaurantBookings.Select(i=>i.BookingCode).Distinct())
                {
                    prods.AddRange(products.Where(i=>i.ProductCode == res));
                }
                prods.OrderBy(i => i.ProductCode);
                {
                    
                    foreach (Products prod in prods)
                        { 
                            priceAdded = 0;


                            foreach (var item in restaurantBookings)
                            {
                                priceAdded += item.PriceOverall;
                                if (item.BookingCode == prod.ProductCode)
                                {
                                    code = item.BookingCode;
                                    multiplicator += item.Multiplicator;
                                    price = item.Price;
                                    pricecomplete += item.PriceOverall;
                                    p = pricecomplete;
                                    desc = item.ProductName;




                                }

                            
                        }

                        ListBoxPrint.Items.Add(String.Format("Produkt:\t\t{0,-10}\t\t{1,5} x {2,5} €\t\t{3,5} €\t\t{4,-10}", code, multiplicator, price, pricecomplete, desc));
                        multiplicator = 0;
                        price = 0;
                        code = "";
                        pricecomplete = 0;
                    //    resBook.Add(new RestaurantBooking { BookingCode = code, Multiplicator = multiplicator, Price = price });


                    }


                    ListBoxPrint.Items.Add("===============================================================================================");
                    ListBoxPrint.Items.Add(String.Format("Gesamteinnahmen:\t\t\t\t\t\t{0} €",priceAdded));
                }
            }
        }
        public List<AllRestaurantBookings> GetAllRestaurantBookings(string connectionString)
        {
            string getAllRestaurantBookingsQuery = "Select * from AllRestaurantBookings where Timestamp<=@Timestamp and Printed=@Printed";
            var bookings = new List<AllRestaurantBookings>();
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
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Printed", false);

                        cmd.CommandText = getAllRestaurantBookingsQuery;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var b = new AllRestaurantBookings
                                {
                                    ID = reader.GetInt32(0),
                                    Text = reader.GetString(1),
                                    Timestamp = reader.GetDateTime(2),
                                    Printed = reader.GetBoolean(3)
                                };
                                bookings.Add(b);
                            }
                            return bookings;

                        }
                    }

                }


            }




            return null;

        }

 /// <summary>
 /// 
 /// </summary>
 /// <param name="connectionString"></param>
 /// <returns></returns>
    List<Products> GetAllCodes(string connectionString)
        {
            string getAllCodesQuery = "";

                getAllCodesQuery = "Select ProductID, ProductCode from Products";

                var codes = new List<Products>();
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

                            cmd.CommandText = getAllCodesQuery;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var b = new Products
                                    {
                                        ProductID = reader.GetInt32(0),
                                        ProductCode = reader.GetString(1),

                                    };
                                    codes.Add(b);
                                }
                                return codes;

                            }
                        }

                    }

                }
                return null;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
     //       restaurantBookings.Clear();
     //       var products = GetAllCodes((App.Current as App).ConnectionString);
     //     var  printList = GetAllRestaurantBookings((App.Current as App).ConnectionString);
            ListBox listBox = new ListBox();
            listBox.Background = new SolidColorBrush(Colors.Transparent);
  /*          foreach (var item in printList)
            {
                restaurantBookings.AddRange(JsonConvert.DeserializeObject<List<RestaurantBooking>>(item.Text));
            }
  */

          //      MessageBox.DisplayDialog("Test", restaurantBookings.Count.ToString());
            if (restaurantBookings.Count > 0)
            {
                listBox.Items.Add(String.Format("Einnahmen von\t{0}\tbis\t{1}",printList.Select(i => i.Timestamp.ToString()).First(), DateTime.Now.ToString()));
                listBox.Items.Add("===============================================================================================");
                foreach (var prod in prods)
                {
                    priceAdded1 = 0;


                    foreach (var item in restaurantBookings)
                    {
                        priceAdded1 += item.PriceOverall;
                        if (item.BookingCode == prod.ProductCode)
                        {
                            code1 = item.BookingCode;
                            multiplicator1 += item.Multiplicator;
                            price1 = item.Price;
                            pricecomplete1 += item.PriceOverall;
                            p1 = pricecomplete1;
                            desc1 = item.ProductName;


                        }

                    }

                    listBox.Items.Add(String.Format("Produkt:\t\t{0,-10}\t\t{1,5} x {2,5} €\t\t{3,5} €\t\t{4, -10}", code1, multiplicator1, price1, pricecomplete1, desc1));
                    multiplicator1 = 0;
                    price1 = 0;
                    code1 = "";
                    pricecomplete1 = 0;
                    //     resBook.Add(new RestaurantBooking { BookingCode = code, Multiplicator = multiplicator, Price = price });


                }


                listBox.Items.Add("===============================================================================================");
                listBox.Items.Add(String.Format("Gesamteinnahmen:\t\t\t\t\t\t{0} €",priceAdded1));
                _printHelper = new PrintHelper(Container);
                var grid = new Grid();
                //grid.Height = 600;
                RowDefinition rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rd);
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
 



            string updateAllBookings = "Update AllRestaurantBookings set Printed=@Printed Where Timestamp<=@Timestamp";
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

                        cmd.Parameters.AddWithValue("@Printed", 1);

                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);


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

            
            Frame.Navigate(typeof(RestaurantPage));
        }
    }
   
}