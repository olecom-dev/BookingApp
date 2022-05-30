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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace BookingApp.Views
{
    public sealed partial class PrintBookingsDialog : ContentDialog
    {
        bool isPrinted = false;
        public PrintBookingsDialog(bool printed)
        {
            this.InitializeComponent();
           
            this.isPrinted = printed;
            if(isPrinted== false)
            {
                tbIsPrinted.Text = "Rechnung anzeigen?";
                cbIsPrinted.Visibility = Visibility.Collapsed;

            }
            else
            {
                tbIsPrinted.Text = "Rechnung ist bereits gedruckt!" + Environment.NewLine + "Wollen Sie erneut ausdrucken?";
                PrimaryButtonText = "OK";

            }
            
           
        }

        private void PrintBookingsDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (isPrinted == true)
            {
                if (cbIsPrinted.IsChecked == false)
                {
                    args.Cancel = true;

                }
            }
        }

        private void PrintBookingsDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void cbIsPrinted_Checked(object sender, RoutedEventArgs e)
        {
            

        }
      
    }
}
