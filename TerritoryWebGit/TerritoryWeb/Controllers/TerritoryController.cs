using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TerritoryWeb;
using TerritoryWeb.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;


namespace TerritoryWeb.Controllers
{
    public class TerritoryController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        // GET: /Territory/
        [AuthLog(Roles = "Admin,Editor,Manager,ReadOnly")]
        public ActionResult Index(string sortOrder)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TerritoryNameSortParam = String.IsNullOrEmpty(sortOrder) ? "TerritoryName_desc" : "";
            ViewBag.DoorsSortParm = sortOrder == "Doors" ? "Doors_desc" : "Doors";
            ViewBag.CitySortParm = sortOrder == "City" ? "City_desc" : "City";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";
            ViewBag.AssignedPublisherIDSortParm = sortOrder == "AssignedPublisherID" ? "AssignedPublisherID_desc" : "AssignedPublisherID";
            ViewBag.CheckedOutSortParm = sortOrder == "CheckedOut" ? "CheckedOut_desc" : "CheckedOut";
            ViewBag.CheckedInSortParm = sortOrder == "CheckedIn" ? "CheckedIn_desc" : "CheckedIn";

            List<Territory> ts = new List<Territory>();
            
            //Sorting
            switch (sortOrder)
            {
                case "TerritoryName_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.TerritoryName).ToList();
                        break;
                    }
                case "Doors":
                    {
                        ts = (from t in db.Territories
                              where t.CongregationID == CongID
                              orderby t.Doors.Count ascending
                              select t).ToList();
                        break;
                    }
                case "Doors_desc":
                    {
                        ts = (from t in db.Territories
                              where t.CongregationID == CongID
                              orderby t.Doors.Count descending
                              select t).ToList();
                        break;
                    }
                case "City":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.City).ToList();
                        break;
                    }
                case "City_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.City).ToList();
                        break;
                    }
                case "Type":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.Type).ToList();
                        break;
                    }
                case "Type_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.Type).ToList();
                        break;
                    }
                case "AssignedPublisherID":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.AssignedPublisherID).ToList();
                        break;
                    }
                case "AssignedPublisherID_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.AssignedPublisherID).ToList();
                        break;
                    }
                case "CheckedOut":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.CheckedOut).ToList();
                        break;
                    }
                case "CheckedOut_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.CheckedOut).ToList();
                        break;
                    }
                case "CheckedIn":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.CheckedIn).ToList();
                        break;
                    }
                case "CheckedIn_desc":
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderByDescending(s => s.CheckedIn).ToList();
                        break;
                    }
                default:
                    {
                        ts = db.Territories.Where(x => x.CongregationID == CongID).OrderBy(s => s.TerritoryName).ToList();
                        break;
                    }
            }


            //Limit territories
            string[] ViewAllGroups = { "Manager", "Admin" };
            bool LimitUser = true;

            for (int i = 0; i < ViewAllGroups.Length; i++)
            {
                if (User.IsInRole(ViewAllGroups[i]))
                {
                    LimitUser = false;
                    break;
                }
            }

            if (LimitUser) //Limit what the user can see to assigned Territories
            {
                ts = ts.Where(x => x.AssignedPublisherID == User.Identity.GetUserId()).ToList();
            }

            return View(ts);
        }

        [AuthLog(Roles = "Admin,Manager,Editor,ReadOnly")]
        public ActionResult ViewTerritories()
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            return View();
        }

        // GET: /Territory/Details/5
        [AuthLog(Roles = "Admin,Manager,Editor,ReadOnly")]
        public ActionResult Details(int? id)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = db.Territories.Find(id);
            if (territory == null || (territory != null && territory.CongregationID != CongID))
            {
                return HttpNotFound();
            }

            //Print if Records in Doors
            ViewBag.PrintEnabled = (territory.Doors.Count > 0 ? "" : "disabled=\"disabled\"");

            return View(territory);
        }

        // GET: /Territory/Create
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult Create()
        {
            ViewBag.TerritoryTypes = new SelectList(db.TerritoryTypes, "Id", "Description");
            return View();
        }

        // POST: /Territory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult Create([Bind(Include = "Id,TerritoryName,City,Type,Notes")] Territory territory, bool createfromgeo, string mapdata = "")
        {
            if (ModelState.IsValid)
            {
                territory.CongregationID = Common.IdentityExtensions.GetCongregationID(User);
                territory.AddedBy = User.Identity.Name;
                territory.Added = DateTime.Now;
                territory.ModifiedBy = User.Identity.Name;
                territory.Modified = DateTime.Now;
                db.Territories.Add(territory);
                db.SaveChanges();

                //if boundaries are present, add
                if (mapdata != "")
                {
                    PointF[] Poly = ParsePolygon(mapdata); //Parse points
                    for (int i = 0; i < Poly.Length; i++)
                    {
                        TerritoryBound t = new TerritoryBound();
                        t.GeoLat = decimal.Parse(Poly[i].X.ToString());
                        t.GeoLong = decimal.Parse(Poly[i].Y.ToString());
                        territory.TerritoryBounds.Add(t); //add to bounds
                    }
                    db.SaveChanges(); //Submit data.
                }


                if (createfromgeo)
                {
                    PointF[] Poly = ParsePolygon(mapdata);
                    List<PointF> points = MapPoints(Poly);

                    List<Door> doors = new List<Door>();
                    
                    //Reverse Geocode
                    foreach (PointF loc in points)
                    {
                        Door d = ReverseGeocodeToDoor(loc);

                        if (d != null && !doors.Any(x => x.Address == d.Address && x.Street == d.Street))
                        {
                            d.TerritoryID = territory.Id;
                            d.AddedBy = User.Identity.Name;
                            d.Added = DateTime.Now;
                            d.ModifiedBy = User.Identity.Name;
                            d.Modified = DateTime.Now;
                            doors.Add(d);
                        }
                    }

                    if (doors.Count > 0)
                    {
                        db.Doors.AddRange(doors);
                        db.SaveChanges();
                    }
                }


                return RedirectToAction("Details", new { id = territory.Id });
            }

            return View(territory);
        }

        // GET: /Territory/Edit/5
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = db.Territories.Find(id);
            if (territory == null || (territory != null && territory.CongregationID != Common.IdentityExtensions.GetCongregationID(User)))
            {
                return HttpNotFound();
            }
            ViewBag.TerritoryTypes = new SelectList(db.TerritoryTypes, "Id", "Description", territory.Type);
            return View(territory);
        }

        // POST: /Territory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult Edit([Bind(Include = "Id,TerritoryName,City,Type,Notes")] Territory territory, string mapdata = "")
        {
            if (ModelState.IsValid)
            {
                Territory t = db.Territories.Find(territory.Id);
                t.Id = territory.Id;
                t.TerritoryName = territory.TerritoryName;
                t.City = territory.City;
                t.Type = territory.Type;
                t.Notes = territory.Notes;

                t.ModifiedBy = User.Identity.Name;
                t.Modified = DateTime.Now;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();

                //if boundaries are present, add
                if (mapdata != "")
                {
                    //Remove previous bounds
                    db.TerritoryBounds.RemoveRange(t.TerritoryBounds);

                    //Add new
                    PointF[] Poly = ParsePolygon(mapdata); //Parse points
                    for (int i = 0; i < Poly.Length; i++)
                    {
                        TerritoryBound tb = new TerritoryBound();
                        tb.GeoLat = decimal.Parse(Poly[i].X.ToString());
                        tb.GeoLong = decimal.Parse(Poly[i].Y.ToString());
                        t.TerritoryBounds.Add(tb); //add to bounds
                    }
                    db.SaveChanges(); //Submit data.
                }

                return RedirectToAction("Details", new { id = t.Id });
            }
            ViewBag.TerritoryTypes = new SelectList(db.TerritoryTypes, "Id", "Description", territory.Type);
            return View(territory);
        }

        // GET: /Territory/Delete/5
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Territory territory = db.Territories.Find(id);
            if (territory == null || (territory != null && territory.CongregationID != Common.IdentityExtensions.GetCongregationID(User)))
            {
                return HttpNotFound();
            }
            return View(territory);
        }

        // POST: /Territory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            Territory territory = db.Territories.Find(id);
            db.Territories.Remove(territory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AuthLog(Roles = "Admin,Manager")]
        public JsonResult DeleteConfirmedJson(int id)
        {
            Territory territory = db.Territories.Find(id);
            if (territory.CongregationID == Common.IdentityExtensions.GetCongregationID(User))
            {
                //Delete doors
                if (territory.Doors.Count > 0)
                {
                    db.Doors.RemoveRange(territory.Doors);
                }

                //Delete Bounds
                if (territory.TerritoryBounds.Count > 0)
                {
                    db.TerritoryBounds.RemoveRange(territory.TerritoryBounds);
                }

                db.Territories.Remove(territory);
                db.SaveChanges();
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        [AuthLog(Roles = "Admin,Manager")]
        public JsonResult CheckOutTerritory(int TerritoryID, string PublisherID)
        {
            if (TerritoryID == 0 || string.IsNullOrWhiteSpace(PublisherID))
            {
                return null;
            }

            AspNetUser p = db.AspNetUsers.SingleOrDefault(x => x.Id == PublisherID);
            if (p == null)
            {
                return null;
            }

            Territory t = db.Territories.Find(TerritoryID);
            if (t.CongregationID != Common.IdentityExtensions.GetCongregationID(User))
            {
                return null;
            }

            t.CheckedIn = null;
            t.LastCheckedInBy = null;
            t.CheckedOut = DateTime.Now;
            t.AssignedPublisherID = PublisherID;
            db.SaveChanges();

            //Send Email
            ApplicationUserManager UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            string TerritoryURL = Url.Action("Details", "Territory", new { id = TerritoryID }, Request.Url.Scheme);
            string body = "You have been assigned a new territory.\r\n\r\nTo View it, please click this link:\r\n " + TerritoryURL;
            UserManager.SendEmail(PublisherID, "Territory Assigned", body);


            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "Admin,Manager,Editor")]
        public JsonResult CheckInTerritory(int TerritoryID)
        {
            if (TerritoryID == 0)
            {
                return null;
            }

            Territory t = db.Territories.Find(TerritoryID);
            if (t.CongregationID != Common.IdentityExtensions.GetCongregationID(User))
            {
                return null;
            }

            t.AssignedPublisherID = null;
            t.CheckedOut = null;
            t.CheckedIn = DateTime.Now;
            t.LastCheckedInBy = User.Identity.GetUserId();
            db.SaveChanges();
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "Admin,Manager")]
        public JsonResult GetListOfPublishers()
        {
            //TODO: Fix Count on query
            Dictionary<string, string> list = new Dictionary<string, string>();
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            List<AspNetUser> plist = (from p in db.AspNetUsers.Include(d => d.Territories)
                                     where p.CongregationID == CongID
                                     orderby p.Territories.Count descending, p.FirstName ascending
                                     select p).ToList();

            plist.ForEach(x => list.Add(x.Id.ToString(), x.FirstName + " (" + x.Territories.Count + ")"));

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [AuthLog(Roles = "Admin,Manager,Editor,ReadOnly")]
        public JsonResult GetTerritoryCord(int TerritoryID)
        {

            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            TerritoryCordinateIDModel tcord = new TerritoryCordinateIDModel();
            var dlist = (from d in db.Doors
                         where d.TerritoryID == TerritoryID && d.GeoLat != null && d.GeoLong != null && d.Territory.CongregationID == CongID
                         orderby d.GeoLat ascending, d.GeoLong ascending
                         select new { d.Id, AddressFull = d.Address + " " + d.Street, d.GeoLat, d.GeoLong }).Distinct().ToList();

            //Get territory bounds
            List<TerritoryBound> tBound = db.TerritoryBounds.Where(x => x.TerritoryID == TerritoryID).ToList();

            //Add Doors to model
            dlist.ForEach(x => tcord.DoorCoordinates.Add(new TerritoryCordinateIDModel.DoorIDModel() { DoorID = x.Id, Address = x.AddressFull, Coordinates = new PointF((float)x.GeoLat, (float)x.GeoLong) }));

            //Add Bounds to model
            tBound.ForEach(x => tcord.HullCoordinates.Add(new PointF((float)x.GeoLat, (float)x.GeoLong)));

            return Json(tcord, JsonRequestBehavior.AllowGet); //Return Model
        }

        [AuthLog(Roles = "Admin,Manager,Editor,ReadOnly")]
        public JsonResult GetTerritories()
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            List<TerritoryCordinateModel> tcord = new List<TerritoryCordinateModel>();

            var territories = db.Territories.Where(x => x.CongregationID == CongID);

            foreach (Territory ter in territories)
            {
                TerritoryCordinateModel t = new TerritoryCordinateModel();
                t.TerritoryID = ter.Id;
                t.TerritoryName = ter.TerritoryName;

                t.HullCoordinates = ter.TerritoryBounds.Select(s => new PointF((float)s.GeoLat, (float)s.GeoLong)).ToList();

                tcord.Add(t);
            }

            return Json(tcord, JsonRequestBehavior.AllowGet); //Return Model
        }

        public async Task<JsonResult> GetTerritoriesMobile()
        {
            int CongID = 1; //TODO: Get rid of this.

            List<TerritoryWeb.Models.MobileModels.TerritoryListModel> tcord = new List<TerritoryWeb.Models.MobileModels.TerritoryListModel>();

            var territories = db.Territories.Where(x => x.CongregationID == CongID);

            await territories.ForEachAsync(x => tcord.Add(new Models.MobileModels.TerritoryListModel() { 
                TerritoryID = x.Id, 
                TerritoryName = x.TerritoryName,
                DoorCount = x.Doors.Count
            }));

            return Json(tcord, JsonRequestBehavior.AllowGet); //Return Model
        }

        public async Task<JsonResult> GetTerritoryDetailsMobile(int TerritoryID)
        {
            int CongID = 1; //TODO: Get rid of this.

            List<TerritoryWeb.Models.MobileModels.TerritoryDetailsModel> tcord = new List<TerritoryWeb.Models.MobileModels.TerritoryDetailsModel>();

            var territories = db.Territories.Where(x => x.CongregationID == CongID && x.Id == TerritoryID);

            //Convert to model
            await territories.ForEachAsync(x => tcord.Add(new Models.MobileModels.TerritoryDetailsModel()
            { 
                TerritoryID = x.Id, 
                TerritoryName = x.TerritoryName,
                City = x.City,
                TerritoryType = x.TerritoryType.Description,
                AssignedPublisher = x.AssignedUser.FullName,
                AssignedDate = x.CheckedOut,
                DoorCount = x.Doors.Count,
                HullCoordinates = x.TerritoryBounds.Select(s => new PointF((float)s.GeoLat, (float)s.GeoLong)).ToList()
            }));

            return Json(tcord, JsonRequestBehavior.AllowGet); //Return Model
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper Functions
        private PointF[] ParsePolygon(string sPoly)
        {
            string[] sPolyArr = sPoly.Split(')');
            List<PointF> Poly = new List<PointF>();
            foreach (string xy in sPolyArr.AsEnumerable())
            {
                string cleanxy = xy.Replace("(", "");
                if (!string.IsNullOrWhiteSpace(cleanxy))
                {
                    string[] xyarr = cleanxy.Split(',');
                    float x;
                    float y;

                    if (xyarr.Length == 2 && float.TryParse(xyarr[0], out x) && float.TryParse(xyarr[1], out y))
                    {
                        PointF p = new PointF(x, y);
                        Poly.Add(p);
                    }
                }
            }
            return Poly.ToArray();
        }

        private List<PointF> MapPoints(PointF[] polygon)
        {
            List<PointF> PointList = new List<PointF>();

            //Get furthest dimentions
            float xplus = -1000;
            float xminus = 1000;
            float yplus = -1000;
            float yminus = 1000;
            foreach (PointF p in polygon.AsEnumerable())
            {
                xplus = (p.X > xplus) ? p.X : xplus;
                xminus = (p.X < xminus) ? p.X : xminus;
                yplus = (p.Y > yplus) ? p.Y : yplus;
                yminus = (p.Y < yminus) ? p.Y : yminus;
            }

            PointF CurrPoint = new PointF(xplus, yplus);
            float ScanRes = 0.00015F;

            while (CurrPoint.Y > yminus)
            {
                if (IsPointInPolygon(polygon, CurrPoint))
                {
                    PointList.Add(CurrPoint);
                }
                CurrPoint.X = CurrPoint.X - ScanRes;

                if (CurrPoint.X < xminus)
                {
                    CurrPoint.X = xplus;
                    CurrPoint.Y = CurrPoint.Y - ScanRes;
                }
            }

            return PointList;
        }

        private bool IsPointInPolygon(PointF[] polygon, PointF point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }

        private Door ReverseGeocodeToDoor(PointF point)
        {
            string coord = point.X.ToString() + "," + point.Y.ToString();

            var address = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + coord + "&key=" + Properties.Settings.Default.GoogleAPIKey;
            var result = new System.Net.WebClient().DownloadString(address);
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var gogResult = jss.Deserialize<GoogleGeoCodeResponse>(result);

            if (gogResult.status == "OK" && gogResult.results.Count() > 0)// && gogResult.results[0].geometry.location_type == "ROOFTOP")
            {
                decimal Lat;
                decimal lng;

                decimal.TryParse(gogResult.results[0].geometry.location.lat, out Lat);
                decimal.TryParse(gogResult.results[0].geometry.location.lng, out lng);


                Door d = new Door();
                d.Address = gogResult.results[0].address_components[0].long_name;
                d.Street = gogResult.results[0].address_components[1].long_name;
                d.PostalCode = gogResult.results[0].address_components[7].long_name;
                d.GeoLat = Lat;
                d.GeoLong = lng;

                return d;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

}