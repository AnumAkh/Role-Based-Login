using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task3.Models;

namespace Task3.ViewModel
{
    public class EventBookingsViewModel
    {
        public Event Event { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}