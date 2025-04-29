using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task3.Models;

namespace Task3.ViewModel
{
    public class AdminDashboardViewModel
    {
        public int TotalEvents { get; set; }
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public List<Event> UpcomingEvents { get; set; }
    }
}