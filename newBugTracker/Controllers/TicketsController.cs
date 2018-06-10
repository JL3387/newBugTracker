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
using System.Threading.Tasks;

namespace newBugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        [Authorize(Roles = "Submitter")]
        public ActionResult SubTickets()
        {
            var userid = User.Identity.GetUserId();
            var subTickets = db.Tickets.Where(u => u.OwnerUserId == userid).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(subTickets.ToList());
        }

        [Authorize(Roles = "Developer")]
        public ActionResult DevTickets()
        {
            var userid = User.Identity.GetUserId();
            var devTickets = db.Tickets.Where(u => u.AssignedToUserId == userid).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(devTickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            if (User.IsInRole("Submitter"))
            {
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                return View();
            }
            return RedirectToAction("Login", "Account");

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId,IsDeleted")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.AssignedToUserId = db.Users.FirstOrDefault(n => n.FirstName == "Unassigned").Id;
                ticket.TicketPriorityId = db.TicketPriorities.First(n => n.Id == 3).Id;
                ticket.TicketStatusId = db.TicketStatus.First(n => n.Id == 1).Id;
                ticket.TicketTypeId = db.TicketTypes.First(n => n.Id == 1).Id;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTime.Now;
                ticket.IsDeleted = false;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("SubTickets");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            if ((User.IsInRole("Admin")) ||
               (User.IsInRole("ProjectManager") && ticket.Project.Users.Any(u => u.Id == user.Id)) ||
               (User.IsInRole("Developer") && ticket.AssignedToUserId == user.Id) ||
               (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id))
            {
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
                //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                return View(ticket);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {

            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                foreach (var prop in typeof(Ticket).GetProperties())
                {
                    if (prop.Name != null && prop.Name.In("Title", "Description", "TicketTypeId", "TicketStatusId", "TicketPropertyId", "TicketPriorityId", "AssignedToUserId"))
                    {
                        var oldInt = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket);
                        var newInt = ticket.GetType().GetProperty(prop.Name).GetValue(ticket);

                        var oldVal = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket).ToString();
                        var newVal = ticket.GetType().GetProperty(prop.Name).GetValue(ticket).ToString();

                        if (prop.Name == "TicketTypeId")
                        {
                            oldVal = db.TicketTypes.Find(oldInt).Name;
                            newVal = db.TicketTypes.Find(newInt).Name;
                        }
                        if (prop.Name == "TicketStatusId")
                        {
                            oldVal = db.TicketStatus.Find(oldInt).Name;
                            newVal = db.TicketStatus.Find(newInt).Name;
                        }
                        if (prop.Name == "TicketPriorityId")
                        {
                            oldVal = db.TicketPriorities.Find(oldInt).Name;
                            newVal = db.TicketPriorities.Find(newInt).Name;
                        }

                        if (oldVal != newVal)
                        {
                            TicketHistory ticketHistory = new TicketHistory()
                            {
                                TicketId = oldTicket.Id,
                                UserId = User.Identity.GetUserId(),
                                Property = prop.Name,
                                OldValue = oldVal,
                                NewValue = newVal,
                                Changed = DateTime.Now
                            };
                            db.TicketHistories.Add(ticketHistory);
                        }
                    }
                }
                ticket.Updated = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("details", new { id = ticket.Id });
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            ticket.IsDeleted = true;
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

        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignTicket(int? id)
        {
            UserRolesHelpers helper = new UserRolesHelpers();
            ProjectHelper projhelper = new ProjectHelper();
            List<ApplicationUser> devList = new List<ApplicationUser>();

            var ticket = db.Tickets.Find(id);
            var projId = ticket.ProjectId;
            var usrList = projhelper.UsersOnProject(projId);
            foreach (var item in usrList)
            {
                if(helper.IsUserInRole(item.Id,"Developer"))
                {
                    var user = db.Users.Find(item.Id);
                    devList.Add(user);
                }
            }
            //var devlist = db.Projects.Where(p => p.Id == projId).Where(p => p.Users ==  (helper.UsersInRole("Developer")));
            //var userList = projhelper.UsersOnProject(ticket.ProjectId).Where(u => u.Roles == helper.UsersInRole("Developer"));
            var users =  helper.UsersInRole("DEVELOPER");
            ViewBag.AssignedToUserId = new SelectList(devList, "Id", "FullName", ticket.AssignedToUserId);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<ActionResult> AssignTicket(Ticket model)
        {
            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == model.Id);
                foreach (var prop in typeof(Ticket).GetProperties())
                {
                    if (prop.Name != null && prop.Name.In("AssignedToUserId"))
                    {
                        var oldVal = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket).ToString();

                        var newVal = model.GetType().GetProperty(prop.Name).GetValue(model).ToString();

                        if (prop.Name == "AssignedToUserId")
                        {
                            oldVal = db.Users.Find(oldVal).FullName;
                            newVal = db.Users.Find(newVal).FullName;
                        }

                        if (oldVal != newVal)
                        {
                            TicketHistory ticketHistory = new TicketHistory()
                            {
                                TicketId = oldTicket.Id,
                                UserId = User.Identity.GetUserId(),
                                Property = prop.Name,
                                OldValue = oldVal,
                                NewValue = newVal,
                                Changed = DateTime.Now
                            };
                            db.TicketHistories.Add(ticketHistory);
                        }



                        var sendeeemail = db.Users.Find(model.AssignedToUserId).Email;
                        var ticket = db.Tickets.Find(model.Id);
                        ticket.AssignedToUserId = model.AssignedToUserId;
                        ticket.TicketStatusId = 2;
                        db.SaveChanges();
                        var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                        try
                        {
                            EmailService ems = new EmailService();
                            IdentityMessage msg = new IdentityMessage();

                            msg.Body = "You have been assigned a new Ticket. " + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\"> NEW TICKET</a>";
                            msg.Destination = sendeeemail;
                            msg.Subject = "You have been assigned a new ticket";

                            TicketNotification ticketNotification = new TicketNotification();
                            ticketNotification.Created = DateTime.Now;
                            ticketNotification.TicketId = ticket.Id;
                            ticketNotification.UserId = User.Identity.GetUserId();
                            ticketNotification.Message = newVal + " was assigned a new Ticket. " + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\"> NEW TICKET</a>";
                            db.TicketNotifications.Add(ticketNotification);
                            db.SaveChanges();

                            await ems.SendMailAsync(msg);


                        }
                        catch (Exception ex)
                        {
                            await Task.FromResult(0);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
