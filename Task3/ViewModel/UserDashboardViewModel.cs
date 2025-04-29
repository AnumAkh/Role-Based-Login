using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task3.Models;

namespace Task3.ViewModel
{
    public class UserDashboardViewModel
    {
        public User User { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
        public List<Booking> PastBookings { get; set; }
    }
}