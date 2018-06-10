using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using newBugTracker.Models;
using newBugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace newBugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projHelper = new ProjectHelper();

        // GET: Projects
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Index()
        {
            return View(db.Projects.Where(p => p.IsDeleted == false).ToList());
        }

        //Get: MyProjects
        public ActionResult MyProjects()
        {
            var userid = User.Identity.GetUserId();
            if (!User.IsInRole("ProjectManager"))
            {
                var myProjects = db.Users.Find(userid).Projects.Where(p => p.IsDeleted == false);
                return View(myProjects);
            }
            var pmProj = db.Projects.Where(p => p.ProjectManager == userid).Where(p => p.IsDeleted == false);

            return View(pmProj);
        }

        //Get: TicketsByProjects
        public ActionResult TicketsByProjects()
        {
            var userid = User.Identity.GetUserId();
            var myProjects = db.Projects.Where(p => p.ProjectManager == userid).Where(p => p.IsDeleted == false).ToList();

            //var tkts = myProjects.SelectMany(p => p.Tickets).Where(p => p.IsDeleted == false);
            //var usrs = myProjects.SelectMany(p => p.Users);
            return View(myProjects);
        }


        private object ListUserProjects(string userid)
        {
            throw new NotImplementedException();
        }

        // GET: Projects/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            UserRolesHelpers helper = new UserRolesHelpers();
            var projManagers = helper.UsersInRole("ProjectManager");
            ViewBag.ProjectManager = new SelectList(projManagers, "Id", "FirstName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name, Created, ProjectManager,IsDeleted, PMName")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.IsDeleted = false;
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                projHelper.AddPM(project.ProjectManager, project.Id);
                return RedirectToAction("Index");
            }
            

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            UserRolesHelpers helper = new UserRolesHelpers();
            var projManagers = helper.UsersInRole("ProjectManager");
            ViewBag.ProjectManager = new SelectList(projManagers, "Id", "FullName");
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Created, Name, ProjectManager, PMName")] Project project)
        {
            if (ModelState.IsValid)
            {
                
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                projHelper.AddPM(project.ProjectManager, project.Id);
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            project.IsDeleted = true;
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
