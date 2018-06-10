using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace newBugTracker.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Exception AddUserToProject (string userId, int projectId)
        {
            try
            {
                var prj = db.Projects.Find(projectId);
                var usr = db.Users.Find(userId);
                prj.Users.Add(usr);
                db.SaveChanges();
                return null;
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        public Exception RemoveUserFromProject(string userId, int projectId)
        {
            try
            {
                var prj = db.Projects.Find(projectId);
                var usr = db.Users.Find(userId);
                prj.Users.Remove(usr);
                db.SaveChanges();
                return null;
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            var usr = db.Users.Find(userId);
            List<Project> ProjUsers = new List<Project>();

            try
            {
                var prj = db.Projects.ToList();
                foreach (var p in prj)
                {
                    if(p.Users.Contains(usr))
                    {
                        ProjUsers.Add(p);
                    }
                }
                return ProjUsers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public bool IsUserOnProject (string userId, int projectId)
        {
            try
            {
                var usr = db.Users.Find(userId);
                var result = db.Projects.Find(projectId).Users.Contains(usr);
                return result;
            }
            catch
            {
                return false;
            }
        }

        public ICollection<ApplicationUser> UsersOnProject (int projectId)
        {
            return db.Projects.Find(projectId).Users.ToList();
        }

        public ICollection<ApplicationUser> UsersNotOnProject (int projectId)
        {
            var usersOnProject = db.Projects.Find(projectId).Users;
            return db.Users.Except(usersOnProject).ToList();
        }

        public void AddPM (string pmId, int projId)
        {
            var proj = db.Projects.Find(projId);
            proj.ProjectManager = pmId;
            proj.PMName = db.Users.Find(pmId).FullName;
            db.SaveChanges();
        }

        public string getPmName(String userId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Find(userId);
            var name = user.FullName;
            return name;
        }
    }
}