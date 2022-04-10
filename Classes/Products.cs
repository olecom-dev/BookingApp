using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public class Products
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string DisplayCode { get { return string.Format("{0}", ProductCode); } }
    }
}
