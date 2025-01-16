using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TerritoryWeb;
using TerritoryWeb.Models;

namespace TerritoryWeb.Controllers
{
    public class LanguageController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        // GET: /Language/
        [AuthLog(Roles = "Admin")]
        public ActionResult Index()
        {
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            return View(db.Languages.Where(x => x.CongregationID == CongID || !x.CongregationID.HasValue).ToList());
        }

        // GET: /Language/Details/5
        [AuthLog(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if ((language == null) || (language != null && language.CongregationID != CongID))
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // GET: /Language/Create
        [AuthLog(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Language/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Description")] Language language)
        {
            if (ModelState.IsValid)
            {
                int CongID = Common.IdentityExtensions.GetCongregationID(User);
                
                //Check that it doesn't exists
                if (db.Languages.Any(x => x.Description == language.Description && x.CongregationID == CongID))
                {
                    ViewBag.ErrorMessage = "Unable to create this language. Language: " + language.Description + " already exists.";
                    return View(language);
                }

                language.CongregationID = CongID;

                db.Languages.Add(language);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(language);
        }

        // GET: /Language/Edit/5
        [AuthLog(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if ((language == null) || (language != null && language.CongregationID != CongID))
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // POST: /Language/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Description")] Language language)
        {
            if (ModelState.IsValid)
            {
                int CongID = Common.IdentityExtensions.GetCongregationID(User);

                //Check that it doesn't exists
                if (db.Languages.Any(x => x.Description == language.Description && x.CongregationID == CongID))
                {
                    ViewBag.ErrorMessage = "Unable to create this language. Language: " + language.Description + " already exists.";
                    return View(language);
                }


                language.CongregationID = CongID;
                db.Entry(language).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(language);
        }

        // GET: /Language/Delete/5
        [AuthLog(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Language language = db.Languages.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if ((language == null) || (language != null && language.CongregationID != CongID))
            {
                return HttpNotFound();
            }
            return View(language);
        }

        // POST: /Language/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Language language = db.Languages.Find(id);
            int CongID = Common.IdentityExtensions.GetCongregationID(User);

            if ((language == null) || (language != null && language.CongregationID != CongID))
            {
                return HttpNotFound();
            }

            //Remove references in data
            var tmp = db.Doors.Where(x => x.LanguageID == language.Id).ToList();
            tmp.ForEach(d => d.LanguageID = null);

            db.Languages.Remove(language);
            db.SaveChanges();
            return RedirectToAction("Index");
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
