using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BookingApp.Classes
{
    public class AlternatingRowListView : ListView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var listViewItem = element as ListViewItem;
            if (listViewItem != null)
            {
                var index = IndexFromContainer(element);

                if (index % 2 == 0)
                {
                    listViewItem.Background = new SolidColorBrush(Windows.UI.Colors.AntiqueWhite);
                }
                else
                {
                    listViewItem.Background = new SolidColorBrush(Windows.UI.Colors.LightGoldenrodYellow);
                }
            }

        }
    }
}
