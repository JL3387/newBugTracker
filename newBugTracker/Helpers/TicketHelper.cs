using Microsoft.AspNet.Identity;
using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace newBugTracker.Helpers
{
    public static class TicketHelper
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        public static List<Ticket> GetProjectTickets(int projectId)
        {
            return db.Tickets.Where(t => t.ProjectId == projectId).ToList();
        }
    }

    public class DeleHelper
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        public void DeleTick(int tickId)
        {
            Ticket ticket = db.Tickets.Find(tickId);
            ticket.IsDeleted = true;
            db.SaveChanges();
        }
    }
}