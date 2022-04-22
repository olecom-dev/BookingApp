using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BookingApp.Classes
{
    public sealed class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;

            // Get the index of a ListViewItem
            int index = listView.IndexFromContainer(item);
            //int indexBIS = listView.IndexFromContainer(item);
            if (index % 2 == 0)
            {
                //https://msdn.microsoft.com/it-it/library/vstudio/system.windows.media.brushes(v=vs.100).aspx
                return new SolidColorBrush(Windows.UI.Colors.LightGray); //new SolidColorBrush(Color.FromArgb(255, 20, 20, 90));

                // return Windows.UI.Xaml.Media.SolidColorBrush.ColorProperty.//Brushes.LightBlue;
            }
            else
            {
                return new SolidColorBrush(Windows.UI.Colors.Beige); //Brushes.Beige;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
