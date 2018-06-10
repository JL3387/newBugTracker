using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using newBugTracker.Models;

namespace newBugTracker.Controllers
{
    public class TicketNotificationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketNotifications
        public ActionResult Index()
        {
            //var ticketNotifications = db.TicketNotifications.Include(t => t.Ticket).Include(t => t.User);
            //return View(ticketNotifications.ToList());
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Admin", "TicketNotifications");
            }
            else if (User.IsInRole("ProjectManager"))
            {
                return RedirectToAction("ProjectManager", "TicketNotifications");
            }
            else if (User.IsInRole("Developer"))
            {
                return RedirectToAction("Developer", "TicketNotifications");
            }
            else if (User.IsInRole("Submitter"))
            {
                return RedirectToAction("Submitter", "TicketNotifications");
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: TicketNotifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketNotification ticketNotification = db.TicketNotifications.Find(id);
            if (ticketNotification == null)
            {
                return HttpNotFound();
            }
            return View(ticketNotification);
        }

        // GET: TicketNotifications/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketNotifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId")] TicketNotification ticketNotification)
        {
            if (ModelState.IsValid)
            {
                db.TicketNotifications.Add(ticketNotification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketNotification.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketNotification.UserId);
            return View(ticketNotification);
        }

        //// GET: TicketNotifications/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketNotification ticketNotification = db.TicketNotifications.Find(id);
        //    if (ticketNotification == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketNotification.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketNotification.UserId);
        //    return View(ticketNotification);
        //}

        //// POST: TicketNotifications/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,TicketId,UserId")] TicketNotification ticketNotification)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(ticketNotification).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketNotification.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketNotification.UserId);
        //    return View(ticketNotification);
        //}

        //// GET: TicketNotifications/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TicketNotification ticketNotification = db.TicketNotifications.Find(id);
        //    if (ticketNotification == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticketNotification);
        //}

        //// POST: TicketNotifications/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TicketNotification ticketNotification = db.TicketNotifications.Find(id);
        //    db.TicketNotifications.Remove(ticketNotification);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            var ticketNotifications = db.TicketNotifications.OrderByDescending(n => n.Created).ToList();
            return View(ticketNotifications.ToList());
        }

        [Authorize(Roles = "ProjectManager")]
        public ActionResult ProjectManager()
        {
            var userId = User.Identity.GetUserId();
            var ticketNotifications = db.TicketNotifications.OrderByDescending(n => n.Created).Where(n => n.Ticket.Project.Users.Any(u => u.Id == userId)).ToList();
            return View(ticketNotifications.ToList());
        }

        [Authorize(Roles = "Developer")]
        public ActionResult Developer()
        {
            var userId = User.Identity.GetUserId();
            var ticketNotifications = db.TicketNotifications.Where(n => n.Ticket.AssignedToUserId == userId).OrderByDescending(x => x.Created).Take(5).ToList();
            return View(ticketNotifications.ToList());
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
