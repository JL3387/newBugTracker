using Microsoft.AspNet.Identity;
using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newBugTracker.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Dashboard
        [Authorize(Roles = "Admin")]
        public ActionResult AdminDashboard()
        {
            var userId = User.Identity.GetUserId();
            var model = new DashboardVM
            {
                Projects = db.Projects.OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                AllTickets = db.Tickets.OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                AllUsers = db.Users.ToList(),
                Notifications = db.TicketNotifications.OrderByDescending(n => n.Created).Take(5).ToList()
            };
            return View(model);
        }

        // GET: Dashboard
        [Authorize(Roles = "ProjectManager")]
        public ActionResult PMDashboard()
        {
            var userId = User.Identity.GetUserId();
            var model = new DashboardVM
            {
                //Tickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                Tickets = db.Projects.Where(p => p.ProjectManager == userId).SelectMany(t => t.Tickets).OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                Projects = db.Projects.Where(p => p.ProjectManager == userId).Where(p => p.IsDeleted == false).ToList(),
                AllTickets = db.Tickets.Where(p => p.IsDeleted == false).ToList(),
                AllUsers = db.Users.ToList(),
                Notifications = db.TicketNotifications.OrderByDescending(n => n.Created).Where(n => n.Ticket.Project.Users.Any(u => u.Id == userId)).Take(5).ToList()
            };
            return View(model);
        }

        // GET: Dashboard
        [Authorize(Roles = "Developer")]
        public ActionResult DevDashboard()
        {
            var userId = User.Identity.GetUserId();
            var model = new DashboardVM
            {
                Tickets = db.Tickets.Where(u => u.AssignedToUserId == userId).OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                Projects = db.Users.Find(userId).Projects.Where(p => p.IsDeleted == false).ToList(),
                AllTickets = db.Tickets.Where(p => p.IsDeleted == false).ToList(),
                AllUsers = db.Users.ToList(),
                Notifications = db.TicketNotifications.Where(n => n.Ticket.AssignedToUserId == userId).OrderByDescending(x => x.Created).Take(5).ToList()
            };
            return View(model);
        }

        // GET: Dashboard
        [Authorize(Roles = "Submitter")]
        public ActionResult SubDashboard()
        {
            var userId = User.Identity.GetUserId();
            var model = new DashboardVM
            {
                Tickets = db.Tickets.Where(u => u.OwnerUserId == userId).OrderByDescending(x => x.Created).Where(p => p.IsDeleted == false).Take(5).ToList(),
                Projects = db.Users.Find(userId).Projects.Where(p => p.IsDeleted == false).ToList(),
                AllTickets = db.Tickets.Where(p => p.IsDeleted == false).ToList(),
                AllUsers = db.Users.ToList(),
            };
            return View(model);
        }

        [Authorize]
        public ActionResult Navigator()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            else if (User.IsInRole("ProjectManager"))
            {
                return RedirectToAction("PMDashboard", "Dashboard");
            }
            else if (User.IsInRole("Developer"))
            {
                return RedirectToAction("DevDashboard", "Dashboard");
            }
            else if (User.IsInRole("Submitter"))
            {
                return RedirectToAction("SubDashboard", "Dashboard");
            }
            return RedirectToAction("Login", "Account");
        }
    }
}