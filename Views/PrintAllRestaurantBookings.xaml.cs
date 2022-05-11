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
        public void Test(object sender, RoutedEventArgs e)
        {
            var products = GetAllCodes((App.Current as App).ConnectionString);
            printList = GetAllRestaurantBookings((App.Current as App).ConnectionString);
            foreach (var item in printList)
            {
                restaurantBookings.AddRange(JsonConvert.DeserializeObject<List<RestaurantBooking>>(item.Text));
            }


         //   MessageBox.DisplayDialog("Test", restaurantBookings.Count.ToString());

            ListBoxPrint.Items.Add("Einnahmen von\t" + printList.Select(i => i.Timestamp.ToString()).First() + "\tbis\t" + DateTime.Now.ToString());
            ListBoxPrint.Items.Add("===============================================================================================");
            foreach (var prod in products)
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

                ListBoxPrint.Items.Add("Produkt:\t\t" + code + "\t\t" + multiplicator + " x " + price + " €\t\t" + pricecomplete + " €\t\t" + desc);
                multiplicator = 0;
                price = 0;
                code = "";
                pricecomplete = 0;
                resBook.Add(new RestaurantBooking { BookingCode = code, Multiplicator = multiplicator, Price = price });


            }


            ListBoxPrint.Items.Add("===============================================================================================");
            ListBoxPrint.Items.Add("Gesamteinnahmen:\t\t\t\t\t" + priceAdded + " €");
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

 
    List<Products> GetAllCodes(string connectionString)
        {
            string getAllCodesQuery = "Select ProductID, ProductCode from Products";
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
        private async void Btn_Print(object sender, RoutedEventArgs e)
        {
            restaurantBookings.Clear();
            var products = GetAllCodes((App.Current as App).ConnectionString);
          var  printList = GetAllRestaurantBookings((App.Current as App).ConnectionString);
            ListBox listBox = new ListBox();
            listBox.Background = new SolidColorBrush(Colors.White);
            foreach (var item in printList)
            {
                restaurantBookings.AddRange(JsonConvert.DeserializeObject<List<RestaurantBooking>>(item.Text));
            }


        //    MessageBox.DisplayDialog("Test", restaurantBookings.Count.ToString());

            listBox.Items.Add("Einnahmen von\t" + printList.Select(i => i.Timestamp.ToString()).First() + "\tbis\t" + DateTime.Now.ToString());
            listBox.Items.Add("===============================================================================================");
            foreach (var prod in products)
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

                listBox.Items.Add("Produkt:\t\t" + code1 + "\t\t" + multiplicator1 + " x " + price1 + " €\t\t" + pricecomplete1 + " €\t\t" + desc1);
                multiplicator1 = 0;
                price1 = 0;
                code1 = "";
                pricecomplete1 = 0;
           //     resBook.Add(new RestaurantBooking { BookingCode = code, Multiplicator = multiplicator, Price = price });


            }


            listBox.Items.Add("===============================================================================================");
            listBox.Items.Add("Gesamteinnahmen:\t\t\t\t\t" + priceAdded1 + " €");
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
      //    _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
      //   _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
            await _printHelper.ShowPrintUIAsync("Rechnung drucken", printHelperOptions);

        }

      
    }
   
}