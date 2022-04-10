using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public class Customer : INotifyPropertyChanged
    {
        public int CustomerID { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string Countrycode { get; set; }
        public string EMailAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string MobilePhone { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
