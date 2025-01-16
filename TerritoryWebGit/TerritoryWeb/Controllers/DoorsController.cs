using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TerritoryWeb;
using TerritoryWeb.Common;
using TerritoryWeb.Models;

namespace TerritoryWeb.Controllers
{
    public class DoorsController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        [AuthLog(Roles = "Admin,Manager,Editor,ReadOnly")]
        public ActionResult Index(int TerritoryID, bool ProxSort = false)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            bool LimitUser = !(User.IsInRole("Admin") || User.IsInRole("Manager"));

            var doors = (from d in db.Doors
                        .Include(d => d.Territory)
                        .Include(d => d.Language)
                        where d.TerritoryID == TerritoryID && d.Territory.CongregationID == CongID
                        orderby d.Street ascending, d.Address ascending
                        select d).ToList();

            ViewBag.ProxEnabled = ProxSort;
            ViewBag.languages = (from l in db.Languages //Get Language List for dropdown.
                                 where !l.CongregationID.HasValue || l.CongregationID == CongID
                                 select new SelectListItem() { Value = l.Id.ToString(), Text = l.Description, Selected = false }).ToList();

            ViewBag.doorcodes = (from l in db.DoorCodes //Get Language List for dropdown.
                                 where l.CongregationID == CongID
                                 select new SelectListItem() { Value = l.Id.ToString(), Text = l.Description, Selected = false }).ToList();

            ViewBag.TerritoryName = db.Territories.Single(x => x.Id == TerritoryID && x.CongregationID == CongID).TerritoryName;

            if (LimitUser) //Limit the dropdown.
            {
                string UserID = Common.IdentityExtensions.GetUserID(User);
                ViewBag.TerritoryID = new SelectList(db.Territories.Where(x => x.Id != TerritoryID && x.CongregationID == CongID && x.AssignedPublisherID == UserID).OrderBy(s => s.TerritoryName), "Id", "TerritoryName");
            }
            else
            {
                ViewBag.TerritoryID = new SelectList(db.Territories.Where(x => x.Id != TerritoryID && x.CongregationID == CongID).OrderBy(s => s.TerritoryName), "Id", "TerritoryName");
            }


            //Proximity Sort
            if (ProxSort && doors.Count() > 0)
            {
                List<Door> DoorList = new List<Door>();
                Door lastAdded = doors[0];
                DoorList.Add(lastAdded);
                Haversine h = new Haversine();
                //Loop Through 
                for (int i = 1; i < doors.Count(); i++)
                {
                    double Prox = 50;
                    Door closest = new Door();
                    TerritoryWeb.Common.Haversine.Position p1 = new TerritoryWeb.Common.Haversine.Position { Latitude = (double)lastAdded.GeoLat, Longitude = (double)lastAdded.GeoLong };

                    //Get the shortest distance (from whatever is not in the DoorList)
                    foreach (Door d in doors.Where(x => !DoorList.Any(s => s.Id == x.Id)))
                    {
                        TerritoryWeb.Common.Haversine.Position p2 = new TerritoryWeb.Common.Haversine.Position { Latitude = (double)d.GeoLat, Longitude = (double)d.GeoLong };
                        //Calculate Proximity
                        double curProx = h.Distance(p1, p2, Haversine.DistanceType.Miles);

                        //If the street is the same, skew distance
                        curProx = (lastAdded.Street == d.Street && curProx > 0.04) ? curProx - 0.04 : curProx;

                        //If distance is shorter, take notes
                        if (Prox > curProx)
                        {
                            Prox = curProx;
                            closest = d;
                        }
                    }

                    //Add to list
                    DoorList.Add(closest);
                    lastAdded = closest;
                }
                doors = DoorList;
            }



            return View(doors);
        }

        // GET: /Doors/Create
        [AuthLog(Roles = "Admin,Manager,Editor")]
        public ActionResult Create(int TerritoryID)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            Territory terr = db.Territories.Single(x => x.Id == TerritoryID && x.CongregationID == CongID);


            List<Territory> terrs = new List<Territory>() { terr };

            ViewBag.TerritoryID = new SelectList(terrs, "Id", "TerritoryName");
            ViewBag.LanguageID = new SelectList(db.Languages.Where(l => !l.CongregationID.HasValue || l.CongregationID == CongID), "Id", "Description");
            ViewBag.CodeID = new SelectList(db.DoorCodes.Where(x => x.CongregationID == CongID), "Id", "Description");
            ViewBag.City = db.Territories.Find(TerritoryID).City;
            return View();
        }

        // POST: /Doors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Manager,Editor")]
        public ActionResult Create([Bind(Include = "Id,TerritoryID,Address,Street,Apartment,LanguageID,Comments,Name,Telephone,PostalCode,CodeID,GeoLat,GeoLong")] Door door)
        {
            if (ModelState.IsValid)
            {
                door.Added = DateTime.Now;
                door.AddedBy = User.Identity.Name;
                door.Modified = DateTime.Now;
                door.ModifiedBy = User.Identity.Name;

                db.Doors.Add(door);
                db.SaveChanges();
                return RedirectToAction("Index", new { TerritoryID = door.TerritoryID });
            }

            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            ViewBag.TerritoryID = new SelectList(db.Territories.Where(x => x.CongregationID == CongID), "Id", "TerritoryName", door.TerritoryID);
            ViewBag.LanguageID = new SelectList(db.Languages.Where(l => !l.CongregationID.HasValue || l.CongregationID == CongID), "Id", "Description", door.LanguageID);
            ViewBag.CodeID = new SelectList(db.DoorCodes.Where(x => x.CongregationID == CongID), "Id", "Description");
            return View(door);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Manager,Editor")]
        public JsonResult InlineEdit([Bind(Include = "Id,TerritoryID,Address,Street,Apartment,LanguageID,Comments,Name,Telephone,PostalCode,CodeID,GeoLat,GeoLong")] Door door)
        {
            if (ModelState.IsValid)
            {
                Door d = db.Doors.Find(door.Id);
                //d.TerritoryID = door.TerritoryID;
                d.Address = door.Address;
                d.Street = door.Street;
                d.Apartment = door.Apartment;
                d.LanguageID = door.LanguageID;
                d.Comments = door.Comments;
                d.Name = door.Name;
                d.Telephone = door.Telephone;
                d.PostalCode = door.PostalCode;
                d.CodeID = door.CodeID;
                d.GeoLat = door.GeoLat;
                d.GeoLong = door.GeoLong;
                d.Modified = DateTime.Now;
                d.ModifiedBy = User.Identity.Name;

                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();
                return Json("OK: Record Updated.");
            }
            return Json("FAILED: Record is not valid.");
        }

        [AuthLog(Roles = "Admin,Manager,Editor")]
        public JsonResult DeleteConfirmedJson(int id)
        {
            Door door = db.Doors.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            if (door.Territory.CongregationID == CongID)
            {
                db.Doors.Remove(door);
                db.SaveChanges();

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetDoorsData(int TerritoryID)
        {
            using (TerritoryWebEntities db = new TerritoryWebEntities())
            {
                return Json((from d in db.Doors 
                             where d.TerritoryID == TerritoryID
                             orderby d.Street ascending, d.Address ascending, d.Apartment ascending
                             select new { 
                                 d.Id, 
                                 d.Address, 
                                 d.Street, 
                                 d.Apartment,
                                 d.GeoLat, 
                                 d.GeoLong, 
                                 d.Territory.TerritoryName, 
                                 d.Comments, 
                                 d.Name, 
                                 d.Telephone, 
                                 Language = d.Language.Description}).ToList(), JsonRequestBehavior.AllowGet);
            }
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
