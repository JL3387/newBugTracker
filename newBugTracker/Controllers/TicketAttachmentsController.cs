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
using newBugTracker.Helpers;
using System.IO;
using System.Threading.Tasks;

namespace newBugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,TicketId,Description,Created,UserId,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (FileValidation.IsWebFriendlyImage(file))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(Path.Combine(Server.MapPath("~/uploads/"), fileName));
                    ticketAttachment.FileUrl = "/uploads/" + fileName;
                }
                var ticket = db.Tickets.Find(ticketAttachment.TicketId);
                var sendeeemail = db.Users.Find(ticket.AssignedToUserId).Email;
                ticketAttachment.UserId = User.Identity.GetUserId();
                ticketAttachment.Created = DateTime.Now;
                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();

                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketAttachment.TicketId }, protocol: Request.Url.Scheme);
                try
                {
                    EmailService ems = new EmailService();
                    IdentityMessage msg = new IdentityMessage();

                    msg.Body = "A new attachment has been added" + " to" + " ticket "+ ticketAttachment.Ticket.Title + " on project " + ticketAttachment.Ticket.Project.Name + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\"> New Attachment</a>";
                    msg.Destination = sendeeemail;
                    msg.Subject = "TicketAttachment";

                    TicketNotification ticketNotification = new TicketNotification();
                    ticketNotification.TicketId = ticketAttachment.TicketId;
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
                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId});
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return RedirectToAction("index", "Tickets");
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
