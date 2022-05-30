using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BookingApp.Classes
{
    public  class DateTimeConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = "";
            if (value == null)
            {
                return null;
            }

            if (parameter == null)
            {
                return value;
            }

            if (value is DateTime timeSpan)
            {
                try
                {
                    if (timeSpan == DateTime.MinValue)
                    {
                        result = "";
                    }
                    else
                    {
                        result = timeSpan.ToString((string)parameter);
                    }
                }
                catch (Exception e)
                {
                    result = "";
                }
            }
            
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
