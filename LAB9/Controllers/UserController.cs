using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LAB9;
using LAB9.ViewModel;

namespace LAB9.Controllers
{
    public class UserController : Controller
    {
        private LogonsEntities db = new LogonsEntities();

        // GET: UserDashboard
        public ActionResult Dashboard()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["Role"]?.ToString() != "user" && Session["Role"]?.ToString() != "User")
            {
                return new HttpStatusCodeResult(403);
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            User user = db.Users.Find(userId);

            UserDetail userDetail = db.UserDetails.FirstOrDefault(ud => ud.UserId == userId);

            var viewModel = new UserProfileViewModel
            {
                User = user,
                UserDetail = userDetail ?? new UserDetail { UserId = userId }
            };

            return View(viewModel);
        }

        // GET: User/Profile - To manage user profile
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            User user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            UserDetail userDetail = db.UserDetails.FirstOrDefault(ud => ud.UserId == userId);

            var viewModel = new UserProfileViewModel
            {
                User = user,
                UserDetail = userDetail ?? new UserDetail { UserId = userId }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(UserProfileViewModel viewModel)
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            if (viewModel.User.UserId != userId)
            {
                return new HttpStatusCodeResult(403); 
            }

            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(userId);
                if (existingUser != null)
                {
                    existingUser.FullName = viewModel.User.FullName;
                    existingUser.Email = viewModel.User.Email;
                }

                var existingDetail = db.UserDetails.FirstOrDefault(ud => ud.UserId == userId);

                if (existingDetail != null)
                {
                    existingDetail.PhoneNumber = viewModel.UserDetail.PhoneNumber;
                    existingDetail.DateOfBirth = viewModel.UserDetail.DateOfBirth;
                    existingDetail.Address = viewModel.UserDetail.Address;
                }
                else
                {
                    viewModel.UserDetail.UserId = userId;
                    db.UserDetails.Add(viewModel.UserDetail);
                }

                db.SaveChanges();

                Session["FullName"] = viewModel.User.FullName;
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Dashboard");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();

                string errorMessage = "Validation errors: " + string.Join(", ", errors);
                ViewBag.ErrorMessage = errorMessage;
                System.Diagnostics.Debug.WriteLine(errorMessage);
            }
            return View(viewModel);
        }



        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            if (id != userId)
            {
                return new HttpStatusCodeResult(403); // Forbidden if trying to view another user
            }

            User user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            UserDetail userDetail = db.UserDetails.FirstOrDefault(ud => ud.UserId == userId);

            var viewModel = new UserProfileViewModel
            {
                User = user,
                UserDetail = userDetail ?? new UserDetail { UserId = userId }
            };

            return View(viewModel);
        }

        // GET: User/ChangePassword
        public ActionResult ChangePassword()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ChangePasswordViewModel());
        }

        // POST: User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                User user = db.Users.Find(userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                if (user.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                    return View(model);
                }

                user.Password = model.NewPassword;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Dashboard");
            }

            return View(model);
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