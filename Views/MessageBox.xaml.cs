using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class MessageBox : ContentDialog
    {
        public MessageBox()
            {
            this.InitializeComponent();
}
       
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
        public static async void DisplayDialog(string title, string content)
        {
           var myStyle = (Style)App.Current.Resources["MyButtonStyle"];
            MessageBox noDialog = new MessageBox()
            {
                Title = title,
                Content = content,
                CloseButtonStyle = myStyle,
                CloseButtonText="OK"
                
                
                
                
                
               

            };
            await noDialog.ShowAsync();
        }
      

}
}

