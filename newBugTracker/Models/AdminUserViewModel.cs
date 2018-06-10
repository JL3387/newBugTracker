using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace newBugTracker.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }


    public class UserRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public ICollection<string> Roles { get; set; }
    }

    public class ProjectUserViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public List<string> SelectedUsers { get; set; }
    }
}