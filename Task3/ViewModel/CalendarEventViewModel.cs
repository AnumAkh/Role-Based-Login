using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task3.ViewModel
{
    public class CalendarEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int RemainingCapacity { get; set; }
    }
}