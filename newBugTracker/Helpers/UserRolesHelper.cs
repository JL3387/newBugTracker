using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace newBugTracker.Helpers
{
    public class UserRolesHelpers
    {
        private UserManager<ApplicationUser> userManager = 
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
  
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.IsInRole(UserId, Role);
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public ICollection<string> ListUserRoles(string UserId)
        {
            return userManager.GetRoles(UserId);
        }

        public bool AddUserToRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.AddToRole(UserId, Role);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }
        
        public bool RemoveUserFromRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.RemoveFromRole(UserId, Role);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }  
        }

        public ICollection<ApplicationUser> UsersInRole(string Role)
        {
            List<ApplicationUser> resultList = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var user in users)
            {
                if (IsUserInRole(user.Id, Role))
                    resultList.Add(user);
            }
            return resultList;
        }

        public ICollection<ApplicationUser> UsersNotInRole(string Role)
        {
            List<ApplicationUser> resultList = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var user in users)
            {
                if (!IsUserInRole(user.Id, Role))
                    resultList.Add(user);
            }
            return resultList;
        }
    }
}