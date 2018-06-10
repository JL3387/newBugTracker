using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using newBugTracker.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;

namespace newBugTracker.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        public ActionResult AssignRoles()
        {
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();
            UserRolesHelpers helper = new UserRolesHelpers();

            var users = db.Users.ToList();
            foreach (var u in users)
            {
                var urvm = new UserRolesViewModel();
                urvm.User = u;
                urvm.Roles = helper.ListUserRoles(u.Id);
                model.Add(urvm);
            }


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        //Get:EditUser
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelpers helper = new UserRolesHelpers();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = new ApplicationUser();
            AdminModel.User.Id = user.Id;
            AdminModel.User.FullName= user.FullName;

            return View(AdminModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser ([Bind(Include = "User,Roles,SelectedRoles")] AdminUserViewModel model)
        {
            var userId = model.User.Id;
            UserRolesHelpers helper = new UserRolesHelpers();
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            {
                if (helper.IsUserInRole(userId, rolermv))
                {
                    helper.RemoveUserFromRole(userId, rolermv);
                }
            }
            foreach (var roleadd in model.SelectedRoles)
            {
                helper.AddUserToRole(userId, roleadd);
            }
            return RedirectToAction("AssignRoles");
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignProjects(string id)
        {
            ProjectHelper projectHelper = new ProjectHelper();
            //Get a list of projects i am already assigned to
            var myProjectsIds = projectHelper.ListUserProjects(id).Select(p => p.Id);
            ViewBag.UserId = id;
            ViewBag.AllProjects = new MultiSelectList(db.Projects, "Id", "Name", myProjectsIds);
            return View();
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignProjects(string UserId, List<int> AllProjects)
        {
            ProjectHelper projectHelper = new ProjectHelper();
            //First use the Project helper to remove this user from all Projects
            foreach (var project in projectHelper.ListUserProjects(UserId))
            {
                projectHelper.RemoveUserFromProject(UserId, project.Id);
            }

            //Next add the user to the selected Projects (AllProjects)
            if (AllProjects != null)
            {
                foreach (var projectId in AllProjects)
                {
                    projectHelper.AddUserToProject(UserId, projectId);
                }
            }
            return RedirectToAction("AssignRoles", "Admin");
        }
    }
}