﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace BookingApp.Classes
{
    public static class MsgBox
    {
        static public async void Show(string mytext)
        {
            var dialog = new MessageDialog(mytext);
           await dialog.ShowAsync();
        }
    }
}
