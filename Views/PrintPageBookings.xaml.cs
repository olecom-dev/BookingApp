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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PrintPageBookings : Page
    {
        Decimal priceOverall = 0;
        public List<RestaurantBooking> restaurantBookings = new List<RestaurantBooking>();
        private PrintHelper _printHelper;
        public PrintPageBookings()
        {
            this.InitializeComponent();
            restaurantBookings.Clear();
            foreach(var items in GlobalVariables.restaurantBookings.Where(i=>i.TableNumber == GlobalVariables.TableNumber)) {
                restaurantBookings.Add(items);
                 
           
                



            }
            

            if (restaurantBookings.Count > 0)
            {
                PrintSampleListView.Header = string.Format("   Rechnung für Tisch: " + GlobalVariables.TableNumber + "{0}   Datum: " + DateTime.Now.Date.ToLongDateString(), Environment.NewLine);
              //  PrintSampleListView.Footer = FooterControl;
            }
            foreach (var item in restaurantBookings)
            {

                priceOverall += item.PriceOverall;
            }
            GlobalVariables.PriceComplete = priceOverall;
            tbPriceComplete.Text = priceOverall.ToString();
        }
        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            
            _printHelper = new PrintHelper(Container);
            for (int i = 0; i < restaurantBookings.Count; i = i + 4)
            {
                var grid = new Grid();
                grid.Height = 600;

                // Main content with layout from data template
                var listView = new ListView();
                listView.Margin = new Thickness(0, 0, 0,700);
                listView.Footer = string.Format("   Gesamtpreis:                                                    {0} €", GlobalVariables.PriceComplete);


                listView.ItemTemplate = CustomPrintTemplate;
                
             
                if (restaurantBookings.Count > 0)
                {
                
                    listView.Header = string.Format("   Rechnung für Tisch: " + GlobalVariables.TableNumber + "{0}   Datum: " + DateTime.Now.Date.ToLongDateString(), Environment.NewLine);




                }


                listView.ItemsSource = restaurantBookings.Skip(0).Take(4);
               
                try
                {
                    
                    grid.Children.Add(listView);
                    



                }
                catch(Exception ex)
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
            foreach(var item in restaurantBookings)
            {
                GlobalVariables.restaurantBookings.Remove(item);
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
