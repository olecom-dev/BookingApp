using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public  class Bills
    {
      public  int BillID { get; set; }
      public  string BillNumber { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
