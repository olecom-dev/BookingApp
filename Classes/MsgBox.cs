using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using BookingApp;

namespace BookingApp.Classes
{
    public static class MsgBox
    {
        public static async void DisplayDialog(string title,string content)
        {
            ContentDialog noDialog = new ContentDialog()
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"

            };
            await noDialog.ShowAsync();
        }
    }
    }
