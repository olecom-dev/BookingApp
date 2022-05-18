using BookingApp.Classes;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Xml.Serialization;
using Windows.Storage;
using System.Globalization;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PrintPageBookings : Page
    {
        //Variabledeclaration
        Decimal priceOverall = 0;
        String BookingCode = "";
        public List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        private PrintHelper _printHelper;
        ListView listView = new ListView();
        List<RestaurantBooking> allBookings = new List<RestaurantBooking>();
        List<AllRestaurantBookings> arb = new List<AllRestaurantBookings>();
        //Constructor
        public PrintPageBookings()
        {
            this.InitializeComponent();
            restaurantBookings.Clear();
            foreach (var items in GlobalVariables.restaurantBookings.Where(i => i.TableNumber == GlobalVariables.TableNumber))
            {
                HyperlinkButton button = new HyperlinkButton();
                button.Name = "BtnCalculate";
                button.Content = "Aktualisieren";
                button.Click += BtnCalculate_Click;
                ListViewCalculate.Items.Add(button);
                restaurantBookings.Add(items);

            }




            if (restaurantBookings.Count > 0)
            {
                PrintListView.Header = string.Format("   Rechnung für Tisch: {1}{0}   Datum: {2}{0}  =====================================================================================", Environment.NewLine, GlobalVariables.TableNumber, DateTime.Now.ToString("f"));
                
            }
            foreach (var item in restaurantBookings)
            {

                priceOverall += item.PriceOverall;
            }
            GlobalVariables.PriceComplete = priceOverall;
            tbPriceComplete.Text = priceOverall.ToString();
        }
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            int multiplicator = GlobalVariables.restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Multiplicator;
            Decimal price = GlobalVariables.restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Price;
            Decimal priceComplete = 0;
            var _MyListView = PrintListView as ListView;
            foreach (var _MyListViewItems in _MyListView.Items)
            {
                var _Container = _MyListView.ContainerFromItem(_MyListViewItems);
                var _Children = AllChildren(_Container);
                var _Name = "tbBookingPriceOverall";
                var _Price = "tbPrice";
                var _Multiplicator = "tbMultiplicator";
                var _BookingCode = "tbBookingCode";
                var _ControlBookingCode = (TextBox)_Children.Find(c => c.Name == _BookingCode);
                // button = (HyperlinkButton)_Children.Find(c => c.Name == "BtnCalculate");
                BookingCode = _ControlBookingCode.Text;
                var _ControlPrice = (TextBox)_Children.Find(c => c.Name == _Price);
                var _ControlMultiplicator = (TextBox)_Children.Find(c => c.Name == _Multiplicator);
                var _Control = (TextBox)_Children.Find(c => c.Name == _Name);
                _Control.Text = (Int32.Parse(_ControlMultiplicator.Text) * Decimal.Parse(_ControlPrice.Text)).ToString();

                GlobalVariables.PriceComplete = Int32.Parse(_ControlMultiplicator.Text) * Decimal.Parse(_ControlPrice.Text);
                GlobalVariables.restaurantBookings.Find(i => i.BookingCode == BookingCode).Multiplicator = Int32.Parse(_ControlMultiplicator.Text);
                GlobalVariables.restaurantBookings.Find(i => i.BookingCode == BookingCode).Price = Decimal.Parse(_ControlPrice.Text);
                //       restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Multiplicator = Int32.Parse(_ControlMultiplicator.Text);
                //       restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Price = Decimal.Parse(_ControlPrice.Text);
                priceComplete += Decimal.Parse(_Control.Text);
                tbPriceComplete.Text = priceComplete.ToString();

                GlobalVariables.PriceComplete = priceComplete;

            }
        }
        private void PrintListView_Tapped(object sender, TappedRoutedEventArgs e)
        {



            // MessageBox.DisplayDialog("Test", _Control.Text);

        }
        private void PrintListItem_Click(object sender, ItemClickEventArgs e)
        {



        }
        private void MySelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }
        /// <summary>
/// Get All Controls in Parent
/// </summary>
/// <param name="parent"></param>
/// <returns>List of Controls</returns>
        private List<Control> AllChildren(DependencyObject parent)
        {
            var _List = new List<Control>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _child = VisualTreeHelper.GetChild(parent, i);
                if (_child is Control)
                {
                    _List.Add((_child as Control));

                }
                _List.AddRange(AllChildren(_child));
            }
            return _List;
        }
        /// <summary>
        /// Print Method for printing listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {

            _printHelper = new PrintHelper(Container);
            for (int i = 0; i < restaurantBookings.Count; i = i + restaurantBookings.Count)
            {
                var grid = new Grid();
                //grid.Height = 600;
                RowDefinition rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rd);
               // ListBox listBox = new ListBox();
                // Main content with layout from data template


                //      listView.Height = 100;
                //   listView.Margin = new Thickness(0, 0, 0,700);
                listView.Footer = string.Format("   =========================================================================================={0}   Gesamtpreis:                                                                                                 {1}     €",Environment.NewLine ,GlobalVariables.PriceComplete);
          //     listBox.Items.Add(string.Format("Rechnung für Tisch:  {0}{1}   Datum:  {2}", Environment.NewLine, DateTime.Now.Date.ToLongDateString()));
                foreach (var item in restaurantBookings)
                {
                    listView.Items.Add(item);
                  //  listBox.Items.Add("Posten:\t" + item.BookingCode + "\t" + item.Multiplicator + " x " + item.Price + " €\t\t" + item.PriceOverall + " €");
                }
                // listBox.Items.Add(string.Format("Gesamtpreis:\t\t\t\t{0} €", GlobalVariables.PriceComplete));
                
                listView.ItemTemplate = CustomPrintTemplate;
                


                if (restaurantBookings.Count > 0)
                {

                    listView.Header = string.Format("   Rechnung für Tisch:\t{0}{1}   Datum:\t\t\t{2}{1}   ==========================================================================================", GlobalVariables.TableNumber,Environment.NewLine,DateTime.Now.ToString("f"));




                }


                

                try
                {

                    grid.Children.Add(listView);




                }
                catch (Exception ex)
                {
                    MessageBox.DisplayDialog("Fehler", ex.Message);
                }
                _printHelper.AddFrameworkElementToPrint(grid);

            }
            var printHelperOptions = new PrintHelperOptions(false);
            printHelperOptions.Orientation = Windows.Graphics.Printing.PrintOrientation.Default;
            printHelperOptions.AddDisplayOption(StandardPrintTaskOptions.Orientation);
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;
            await _printHelper.ShowPrintUIAsync("Rechnung drucken", printHelperOptions);

        }
        /// <summary>
        /// Function if Printing succeeded. Json serialize bill and save in Database. Delete TableNr row from Database
        /// </summary>
        private void PrintHelper_OnPrintSucceeded()
        {
            _printHelper.Dispose();
            MessageBox.DisplayDialog("OK", "Rechnung gedruckt.");
    
            GlobalVariables.restaurantBookings.OrderBy(u => u.TableNumber);

            string json = JsonConvert.SerializeObject(restaurantBookings);


            foreach (var item in restaurantBookings)
            {
                GlobalVariables.restaurantBookings.Remove(item);
            }
            string insertAllBookings = "Insert into AllRestaurantBookings (Text, Timestamp, Printed) values (@Text, @Timestamp, @Printed)";
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
                        cmd.CommandText = insertAllBookings;

                        cmd.Parameters.AddWithValue("@Text", json);

                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Printed", 0);


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
                string deleteTable = "Delete from Tables where Tablenumber=@Tablenumber";
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
                        cmd.CommandText = deleteTable;

                        cmd.Parameters.AddWithValue("@TableNumber", GlobalVariables.TableNumber);

          


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
        /// <summary>
        /// Function if Printing command failed
        /// </summary>
        private void PrintHelper_OnPrintFailed()
        {
            _printHelper.Dispose();
            MessageBox.DisplayDialog("Fehler", "Rechnung konnte nicht gedruckt werden.");
      
        }

     

    }
}
