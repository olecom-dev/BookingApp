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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PrintPageBookings : Page
    {
        Decimal priceOverall = 0;
        String BookingCode = "";
        public List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        private PrintHelper _printHelper;
        ListView listView = new ListView();
        List<RestaurantBooking> allBookings = new List<RestaurantBooking>();
        List<AllRestaurantBookings> arb = new List<AllRestaurantBookings>();
        public PrintPageBookings()
        {
            this.InitializeComponent();
            restaurantBookings.Clear();
            foreach (var items in GlobalVariables.restaurantBookings.Where(i => i.TableNumber == GlobalVariables.TableNumber))
            {

                restaurantBookings.Add(items);

            }




            if (restaurantBookings.Count > 0)
            {
                PrintListView.Header = string.Format("   Rechnung für Tisch: " + GlobalVariables.TableNumber + "{0}   Datum: " + DateTime.Now.Date.ToLongDateString(), Environment.NewLine);
                //  PrintSampleListView.Footer = FooterControl;
            }
            foreach (var item in restaurantBookings)
            {

                priceOverall += item.PriceOverall;
            }
            GlobalVariables.PriceComplete = priceOverall;
            tbPriceComplete.Text = priceOverall.ToString();
        }
        private void PrintListView_Tapped(object sender, TappedRoutedEventArgs e)
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
                BookingCode = _ControlBookingCode.Text;
                var _ControlPrice = (TextBox)_Children.Find(c => c.Name == _Price);
                var _ControlMultiplicator = (TextBox)_Children.Find(c => c.Name == _Multiplicator);
                var _Control = (TextBox)_Children.Find(c => c.Name == _Name);
                _Control.Text = (Int32.Parse(_ControlMultiplicator.Text) * Decimal.Parse(_ControlPrice.Text)).ToString();

                GlobalVariables.PriceComplete = Int32.Parse(_ControlMultiplicator.Text) * Decimal.Parse(_ControlPrice.Text);
                GlobalVariables.restaurantBookings.Find(i => i.BookingCode == BookingCode).Multiplicator = Int32.Parse(_ControlMultiplicator.Text);
                GlobalVariables.restaurantBookings.Find(i => i.BookingCode == BookingCode).Price = Decimal.Parse(_ControlPrice.Text);
                //     restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Multiplicator = Int32.Parse(_ControlMultiplicator.Text);
                //       restaurantBookings.Find(i => i.TableNumber == GlobalVariables.TableNumber).Price = Decimal.Parse(_ControlPrice.Text);
                priceComplete += Decimal.Parse(_Control.Text);
                tbPriceComplete.Text = priceComplete.ToString();
                GlobalVariables.PriceComplete = priceComplete;

            }


            // MessageBox.DisplayDialog("Test", _Control.Text);

        }
        private void PrintListItem_Click(object sender, ItemClickEventArgs e)
        {



        }


        private void MySelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

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

                // Main content with layout from data template


                //      listView.Height = 100;
                //   listView.Margin = new Thickness(0, 0, 0,700);
                listView.Footer = string.Format("   Gesamtpreis:                                                                                             {0}     €", GlobalVariables.PriceComplete);
                foreach (var item in restaurantBookings)
                {
                    listView.Items.Add(item);
                }

                listView.ItemTemplate = CustomPrintTemplate;


                if (restaurantBookings.Count > 0)
                {

                    listView.Header = string.Format("   Rechnung für Tisch: " + GlobalVariables.TableNumber + "{0}   Datum: " + DateTime.Now.Date.ToLongDateString(), Environment.NewLine);




                }



                // listView.ItemsSource = GlobalVariables.restaurantBookings;

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
        private async void PrintHelper_OnPrintSucceeded()
        {
            _printHelper.Dispose();

            var dialog = new MessageDialog("Rechnung gedruckt.");
            await dialog.ShowAsync();
            GlobalVariables.restaurantBookings.OrderBy(u => u.TableNumber);

            string json = JsonConvert.SerializeObject(restaurantBookings);


            foreach (var item in restaurantBookings)
            {
                GlobalVariables.restaurantBookings.Remove(item);
            }
            string insertAllBookings = "Insert into AllRestaurantBookings (Text, Timestamp) values (@Text, @Timestamp)";
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
    


        private async void PrintHelper_OnPrintFailed()
        {
            _printHelper.Dispose();
            var dialog = new MessageDialog("Rechnung konnte nicht gedruckt werden.");
            await dialog.ShowAsync();
        }
        DependencyObject FindElementInItemsControlItemAtIndex(ItemsControl itemsControl,
                                                                int itemOfIndexToFind,
                                                                string nameOfControlToFind)
        {
            if (itemOfIndexToFind >= itemsControl.Items.Count) return null;

            DependencyObject depObj = null;
            object o = itemsControl.Items[itemOfIndexToFind];
            if (o != null)
            {
                var item = itemsControl.ContainerFromItem(o);
                if (item != null)
                {
                    //GridViewItem it = item as GridViewItem;
                    //var i = it.FindName(nameOfControlToFind);
                    depObj = GetVisualTreeChild(item, nameOfControlToFind);
                    return depObj;
                }
            }
            return null;
        }

        DependencyObject GetVisualTreeChild(DependencyObject obj, String name)
        {
            DependencyObject dependencyObject = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < childrenCount; i++)
            {
                var oChild = VisualTreeHelper.GetChild(obj, i);
                var childElement = oChild as FrameworkElement;
                if (childElement != null)
                {
                    //Code to take care of Paragraph/Run
                    if (childElement is RichTextBlock || childElement is TextBlock)
                    {
                        dependencyObject = childElement.FindName(name) as DependencyObject;
                        if (dependencyObject != null)
                            return dependencyObject;
                    }

                    if (childElement.Name == name)
                    {
                        return childElement;
                    }
                }
                dependencyObject = GetVisualTreeChild(oChild, name);
                if (dependencyObject != null)
                    return dependencyObject;
            }
            return dependencyObject;
        }
    }
}
