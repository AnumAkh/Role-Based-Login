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
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private EventManagerEntities db = new EventManagerEntities();

        // GET: User
        public ActionResult Dashboard()
        {
            int userId = (int)Session["UserId"];

            var user = db.Users.Find(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var dashboardModel = new UserDashboardViewModel
            {
                User = user,
                UpcomingBookings = db.Bookings
                    .Where(b => b.UserId == userId && b.Event.EventDate >= DateTime.Today)
                    .OrderBy(b => b.Event.EventDate)
                    .Include(b => b.Event)
                    .ToList(),
                PastBookings = db.Bookings
                    .Where(b => b.UserId == userId && b.Event.EventDate < DateTime.Today)
                    .OrderByDescending(b => b.Event.EventDate)
                    .Include(b => b.Event)
                    .ToList()
            };

            return View(dashboardModel);
        }

        // GET: User/BookEvent/5
        public ActionResult BookEvent(int id)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            int userId = (int)Session["UserId"];

            // Check if user has already booked this event
            bool alreadyBooked = db.Bookings.Any(b => b.UserId == userId && b.EventId == id);
            if (alreadyBooked)
            {
                TempData["Error"] = "You have already booked this event!";
                return RedirectToAction("Details", "Event", new { id = id });
            }

            // Check if event is at capacity
            int bookedSeats = evt.Bookings.Count();
            if (bookedSeats >= evt.Capacity)
            {
                TempData["Error"] = "This event is at full capacity!";
                return RedirectToAction("Details", "Event", new { id = id });
            }

            // Create booking confirmation view model
            var model = new BookingConfirmationViewModel
            {
                Event = evt,
                RemainingCapacity = evt.Capacity - bookedSeats
            };

            return View(model);
        }

        // POST: User/BookEvent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookEvent(int id, BookingConfirmationViewModel model)
        {
            var evt = db.Events.Find(id);
            if (evt == null)
            {
                return HttpNotFound();
            }

            int userId = (int)Session["UserId"];

            // Check if user has already booked this event (double-check to prevent race conditions)
            bool alreadyBooked = db.Bookings.Any(b => b.UserId == userId && b.EventId == id);
            if (alreadyBooked)
            {
                TempData["Error"] = "You have already booked this event!";
                return RedirectToAction("Details", "Event", new { id = id });
            }

            // Check if event is at capacity (double-check to prevent race conditions)
            int bookedSeats = db.Bookings.Count(b => b.EventId == id);
            if (bookedSeats >= evt.Capacity)
            {
                TempData["Error"] = "This event is at full capacity!";
                return RedirectToAction("Details", "Event", new { id = id });
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                EventId = id,
                BookingDate = DateTime.Now
            };

            db.Bookings.Add(booking);
            db.SaveChanges();

            TempData["Success"] = "Event booked successfully!";
            return RedirectToAction("Dashboard");
        }

        // POST: User/CancelBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(int id)
        {
            int userId = (int)Session["UserId"];
            var booking = db.Bookings.FirstOrDefault(b => b.BookingId == id && b.UserId == userId);

            if (booking == null)
            {
                return HttpNotFound();
            }

            // Check if event is in the past
            if (booking.Event.EventDate < DateTime.Today)
            {
                TempData["Error"] = "Cannot cancel bookings for past events!";
                return RedirectToAction("Dashboard");
            }

            db.Bookings.Remove(booking);
            db.SaveChanges();

            TempData["Success"] = "Booking cancelled successfully!";
            return RedirectToAction("Dashboard");
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