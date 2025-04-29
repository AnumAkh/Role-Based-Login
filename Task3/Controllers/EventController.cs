using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Task3.Models;
using Task3.ViewModel;

namespace Task3.Controllers
{
    public class EventController : Controller
    {
        private EventManagerEntities db = new EventManagerEntities();

        // GET: Event
        public ActionResult Index()
        {
            var events = db.Events.ToList();
            return View(events);
        }

        // GET: Event/Details/5
        public ActionResult Details(int id)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            // Calculate remaining capacity
            int bookedSeats = evt.Bookings.Count();
            int remainingCapacity = evt.Capacity - bookedSeats;

            var model = new EventDetailsViewModel
            {
                Event = evt,
                RemainingCapacity = remainingCapacity,
                IsBooked = false
            };

            // Check if current user has booked this event
            if (Session["UserId"] != null)
            {
                int userId = (int)Session["UserId"];
                model.IsBooked = evt.Bookings.Any(b => b.UserId == userId);
            }

            return View(model);
        }

        // GET: Event/Calendar
        public ActionResult Calendar()
        {
            var events = db.Events
                .Select(e => new CalendarEventViewModel
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Start = e.EventDate.Add(e.StartTime),
                    End = e.EventDate.Add(e.EndTime),
                    RemainingCapacity = e.Capacity - e.Bookings.Count
                })
                .ToList();

            return View(events);
        }

        // For AJAX calendar data
        [HttpGet]
        public JsonResult GetCalendarEvents()
        {
            var events = db.Events
                .Select(e => new CalendarEventViewModel
                {
                    Id = e.EventId,
                    Title = e.Title,
                    Start = e.EventDate.Add(e.StartTime),
                    End = e.EventDate.Add(e.EndTime),
                    RemainingCapacity = e.Capacity - e.Bookings.Count
                })
                .ToList();

            return Json(events, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}