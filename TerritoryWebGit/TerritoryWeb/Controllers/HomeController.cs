using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TerritoryWeb.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TerritoryWeb.Controllers
{
    public class HomeController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        public ActionResult Index()
        {
            HomeScreenModel hsm = new HomeScreenModel();
            hsm.CheckedInTerritories = new List<HomeScreenModel.CheckedInTerritoriesModel>();
            hsm.TerritoriesAdded = new List<HomeScreenModel.TerritoriesModifiedAdded>();
            hsm.TerritoriesModified = new List<HomeScreenModel.TerritoriesModifiedAdded>();

            if (Request.IsAuthenticated)
            {
                string[] Allowed = { "Manager", "Admin" };

                for (int i = 0; i < Allowed.Length; i++)
                {
                    if (User.IsInRole(Allowed[i]))
                    {
            
                        int CongID = Common.IdentityExtensions.GetCongregationID(User);
                        DateTime lastDay = DateTime.Now.AddDays(-30).Date;

                        hsm.ShowDash = true; //If any allowed, then show
                        hsm.CheckedInTerritories = (from c in db.Territories
                                                    where c.CongregationID == CongID && c.CheckedIn.HasValue
                                                    orderby c.CheckedIn descending
                                                    select new TerritoryWeb.Models.HomeScreenModel.CheckedInTerritoriesModel() 
                                                    { TerritoryID = c.Id, Territory = c.TerritoryName, CheckedInTime = c.CheckedIn.Value }).Take(5).ToList();
                        hsm.TerritoriesAdded = (from t in db.Territories
                                                where t.CongregationID == CongID && t.Doors.Any(x => x.Added >= lastDay)
                                                select new TerritoryWeb.Models.HomeScreenModel.TerritoriesModifiedAdded() 
                                                { TerritoryID = t.Id, Territory = t.TerritoryName, count = t.Doors.Count(), AddedModDate = t.Doors.OrderByDescending(s => s.Added).FirstOrDefault().Added }).OrderByDescending(s => s.AddedModDate).ToList();

                        hsm.TerritoriesModified = (from t in db.Territories
                                                   where t.CongregationID == CongID && t.Doors.Any(x => x.Modified >= lastDay)
                                                   select new TerritoryWeb.Models.HomeScreenModel.TerritoriesModifiedAdded() 
                                                   { TerritoryID = t.Id, Territory = t.TerritoryName, count = t.Doors.Count(), AddedModDate = t.Doors.OrderByDescending(s => s.Modified).FirstOrDefault().Modified }).OrderByDescending(s => s.AddedModDate).ToList();

                        break;
                    }
                }
            }
            return View(hsm);
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
    }
}