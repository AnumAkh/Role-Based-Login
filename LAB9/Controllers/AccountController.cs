using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using LAB9;
using LAB9.ViewModel;

namespace LAB9.Controllers
{
    public class AccountController : Controller
    {
        private LogonsEntities db = new LogonsEntities();

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null && user.Password == model.Password) 
                {
                    FormsAuthentication.SetAuthCookie(user.Email, model.RememberMe);

                    Session["UserId"] = user.UserId;
                    Session["UserEmail"] = user.Email;
                    Session["FullName"] = user.FullName;
                    Session["Role"] = user.Role;

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }

                    if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "User");
                    }
                }

                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View(model);
                }

                var user = new User
                {
                    Email = model.Email,
                    Password = model.Password,
                    FullName = model.FullName,
                    Role = "admin",
                    CreatedAt = DateTime.Now,
                };
                db.Users.Add(user);
                db.SaveChanges();

                FormsAuthentication.SetAuthCookie(user.Email, false);

                Session["UserId"] = user.UserId;
                Session["UserEmail"] = user.Email;
                Session["FullName"] = user.FullName;
                Session["Role"] = user.Role;

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
