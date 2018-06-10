using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newBugTracker.Models
{
    public class DashboardVM
    {
        public ApplicationUser User { get; set; }
        public List<ApplicationUser> AllUsers { get; set; }
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<Ticket> AllTickets { get; set; }
        public List<TicketNotification> Notifications { get; set; }

        //public DashboardVM()
        //{
        //    Projects = new List<Project>();
        //    Tickets = new List<Ticket>();
        //}
    }
}