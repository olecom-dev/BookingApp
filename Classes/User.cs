using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Classes
{
    public class User
    {
        public int ID { get; set; }
        public string Firstname{get; set;}
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string EMail { get; set; }
        public DateTime LastLogin { get; set; }
        public int Counter { get; set; }
    }
}
