using Microsoft.AspNet.Identity;
using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newBugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    var userId = User.Identity.GetUserId();
            //    var model = new DashboardVM
            //    {
            //        Tickets = db.Tickets.Where(u => u.AssignedToUserId == userId).ToList(),
            //        Projects = db.Users.Find(userId).Projects.ToList(),
            //        AllTickets = db.Tickets.ToList(),
            //        AllUsers = db.Users.ToList()
            //    };
            //    return View(model);
            //    //return View();
            //}
            //return View("Landing");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Landing()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}