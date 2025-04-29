using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Task3.Models;
using Task3.ViewModel;

namespace EventBookingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private EventManagerEntities db = new EventManagerEntities();

        // GET: Admin
        public ActionResult Dashboard()
        {
            var dashboardModel = new AdminDashboardViewModel
            {
                TotalEvents = db.Events.Count(),
                TotalUsers = db.Users.Count(u => u.Role != "Admin"),
                TotalBookings = db.Bookings.Count(),
                UpcomingEvents = db.Events.Where(e => e.EventDate >= DateTime.Today).OrderBy(e => e.EventDate).Take(5).ToList()
            };

            return View(dashboardModel);
        }

        // GET: Admin/Events
        public ActionResult Events()
        {
            var events = db.Events.OrderByDescending(e => e.CreatedAt).ToList();
            return View(events);
        }

        // GET: Admin/CreateEvent
        public ActionResult CreateEvent()
        {
            return View();
        }

        // POST: Admin/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var evt = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    EventDate = model.EventDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Location = model.Location,
                    Capacity = model.Capacity,
                    CreatedAt = DateTime.Now
                };

                db.Events.Add(evt);
                db.SaveChanges();
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Events");
            }

            return View(model);
        }

        // GET: Admin/EditEvent/5
        public ActionResult EditEvent(int id)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            var model = new EventViewModel
            {
                EventId = evt.EventId,
                Title = evt.Title,
                Description = evt.Description,
                EventDate = evt.EventDate,
                StartTime = evt.StartTime,
                EndTime = evt.EndTime,
                Location = evt.Location,
                Capacity = evt.Capacity
            };

            return View(model);
        }

        // POST: Admin/EditEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEvent(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var evt = db.Events.Find(model.EventId);
                if (evt == null)
                {
                    return HttpNotFound();
                }

                evt.Title = model.Title;
                evt.Description = model.Description;
                evt.EventDate = model.EventDate;
                evt.StartTime = model.StartTime;
                evt.EndTime = model.EndTime;
                evt.Location = model.Location;
                evt.Capacity = model.Capacity;

                db.SaveChanges();
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Events");
            }

            return View(model);
        }

        // POST: Admin/DeleteEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteEvent(int id)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            // Check if event has bookings
            if (evt.Bookings.Any())
            {
                TempData["Error"] = "Cannot delete an event with existing bookings!";
                return RedirectToAction("Events");
            }

            db.Events.Remove(evt);
            db.SaveChanges();
            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction("Events");
        }

        // GET: Admin/ManageBookings/5
        public ActionResult ManageBookings(int id)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            var bookings = evt.Bookings.ToList();
            var model = new EventBookingsViewModel
            {
                Event = evt,
                Bookings = bookings
            };

            return View(model);
        }

        // POST: Admin/CancelBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(int id)
        {
            var booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }

            int eventId = booking.EventId;
            db.Bookings.Remove(booking);
            db.SaveChanges();
            TempData["Success"] = "Booking cancelled successfully!";
            return RedirectToAction("ManageBookings", new { id = eventId });
        }

        // GET: Admin/Users
        public ActionResult Users()
        {
            var users = db.Users.Where(u => u.Role != "Admin").ToList();
            return View(users);
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