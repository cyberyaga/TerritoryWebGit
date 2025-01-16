using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TerritoryWeb.Models;
using System.Security;

namespace TerritoryWeb.Common
{
    public static class IdentityExtensions
    {
        public static int GetCongregationID(IPrincipal CurUser)
        {
            int CongID = 0;
            if (CurUser.Identity.IsAuthenticated)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(CurUser.Identity.GetUserId());

                return currentUser.CongregationID.HasValue ? currentUser.CongregationID.Value : 0;
            }
            return CongID;
        }

        public static string GetUserID(IPrincipal CurUser)
        {
            return CurUser.Identity.GetUserId();
        }

        public static string GetLanguage(IPrincipal CurUser)
        {
            string Language = "en";
            if (CurUser.Identity.IsAuthenticated)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(CurUser.Identity.GetUserId());
                return currentUser.Language;
            }
            return Language;
        }

        public static List<string> GetRoles(IPrincipal CurUser)
        {
            List<string> r = new List<string>();
            if (CurUser.Identity.IsAuthenticated)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(CurUser.Identity.GetUserId());
                return manager.GetRoles(currentUser.Id).ToList();                
            }
            return r;
        }

        public static int GetPermission(IPrincipal CurUser)
        {
            int p = 0;
            if (CurUser.Identity.IsAuthenticated)
            {
                var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var currentUser = manager.FindById(CurUser.Identity.GetUserId());
                if (currentUser.TypeID.HasValue)
                {
                    p = currentUser.TypeID.Value;
                }
            }
            return p;
        }
    }
}
