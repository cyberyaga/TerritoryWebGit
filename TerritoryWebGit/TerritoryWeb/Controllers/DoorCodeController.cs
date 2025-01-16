using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TerritoryWeb.Models;

namespace TerritoryWeb.Controllers
{
    public class DoorCodeController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        // GET: DoorCode
        public ActionResult Index()
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            var model = db.DoorCodes.Where(x => x.CongregationID == CongID);
            return View(model);
        }

        // GET: DoorCode/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoorCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Description")] DoorCode dc)
        {
            if (ModelState.IsValid)
            {
                int CongID = Common.IdentityExtensions.GetCongregationID(User);

                //Check that it doesn't exists
                if (db.DoorCodes.Any(x => x.Description == dc.Description && x.CongregationID == CongID))
                {
                    ViewBag.ErrorMessage = "Unable to create this Door Code. Value: " + dc.Description + " already exists.";
                    return View(dc);
                }

                dc.CongregationID = CongID;

                db.DoorCodes.Add(dc);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(dc);
        }

        // GET: DoorCode/Edit/5
        public ActionResult Edit(int id)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            var dc = db.DoorCodes.SingleOrDefault(d => d.Id == id && d.CongregationID == CongID);

            if (dc != null)
            {
                return View(dc);
            }
            else
            {
                return HttpNotFound();
            }

        }

        // POST: DoorCode/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id,Description")] DoorCode dc)
        {
            if (ModelState.IsValid)
            {
                int CongID = Common.IdentityExtensions.GetCongregationID(User);

                //Check that it doesn't exists
                if (db.DoorCodes.Any(x => x.Description == dc.Description && x.Id != dc.Id && x.CongregationID == CongID))
                {
                    ViewBag.ErrorMessage = "Unable to create this Door Code. Value: " + dc.Description + " already exists.";
                    return View(dc);
                }

                dc.CongregationID = CongID;
                db.Entry(dc).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(dc);
        }

        // GET: DoorCode/Delete/5
        public ActionResult Delete(int id)
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);
            var dc = db.DoorCodes.SingleOrDefault(d => d.Id == id && d.CongregationID == CongID);

            if (dc != null)
            {
                return View(dc);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: DoorCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoorCode dc = db.DoorCodes.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if (dc != null && dc.CongregationID != CongID)
            {
                return HttpNotFound();
            }

            //Remove references in data
            var tmp = db.Doors.Where(x => x.CodeID == dc.Id).ToList();
            tmp.ForEach(d => d.CodeID = null);

            db.DoorCodes.Remove(dc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
