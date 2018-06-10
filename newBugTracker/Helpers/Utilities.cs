using newBugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;

namespace newBugTracker.Helpers
{
    public static class Utilities
    {
        public static bool In(this string source, params string[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source, StringComparer.OrdinalIgnoreCase);
        }

        public static string GetFullName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "FullName");
            if(claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }
    }

}