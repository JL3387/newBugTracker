using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using newBugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace newBugTracker.Controllers
{
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(ticketComment.TicketId);
                var sendeeemail = db.Users.Find(ticket.AssignedToUserId).Email;
                ticketComment.UserId = User.Identity.GetUserId();
                ticketComment.Created = DateTime.Now;
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                try
                {
                    EmailService ems = new EmailService();
                    IdentityMessage msg = new IdentityMessage();

                    msg.Body = "A new comment has been added. " + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\"> New Commment</a>";
                    msg.Destination = sendeeemail;
                    msg.Subject = "New Comment";

                    TicketNotification ticketNotification = new TicketNotification();
                    ticketNotification.TicketId = ticketComment.TicketId;
                    ticketNotification.Created = DateTime.Now;
                    ticketNotification.UserId = User.Identity.GetUserId();
                    ticketNotification.Message = msg.Body;
                    db.TicketNotifications.Add(ticketNotification);
                    db.SaveChanges();

                    await ems.SendMailAsync(msg);
                }
                catch (Exception ex)
                {
                    await Task.FromResult(0);
                }
                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
        }

        // GET: TicketComments/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Comment,Created,TicketId,UserId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
