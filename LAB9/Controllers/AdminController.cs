using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LAB9;

namespace LAB9.Controllers
{
    public class AdminController : Controller
    {
        private LogonsEntities db = new LogonsEntities();

        // GET: Admin Dashboard
        public ActionResult Dashboard()
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            var users = db.Users.ToList();
            return View(users);
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,FullName,Email,Password,Role,CreatedAt")] User user)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            if (ModelState.IsValid)
            {
                // Set creation date to current time if not provided
                if (user.CreatedAt == null || user.CreatedAt == default(DateTime))
                {
                    user.CreatedAt = DateTime.Now;
                }

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View(user);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,FullName,Email,Password,Role,CreatedAt")] User user)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View(user);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["Role"]?.ToString() != "admin" && Session["Role"]?.ToString() != "Admin")
            {
                return new HttpStatusCodeResult(403);
            }

            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
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