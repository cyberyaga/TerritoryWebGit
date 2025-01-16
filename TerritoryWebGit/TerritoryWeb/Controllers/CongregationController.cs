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
    public class CongregationController : Controller
    {
        private TerritoryWebEntities db = new TerritoryWebEntities();

        // GET: /Congregation/
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View(db.Congregations.ToList());
        }

        // GET: /Congregation/Details/5
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Congregation congregation = db.Congregations.Find(id);
            if (congregation == null)
            {
                return HttpNotFound();
            }
            return View(congregation);
        }

        // GET: /Congregation/Create
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Congregation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Create([Bind(Include = "Id,Description")] Congregation congregation)
        {
            if (ModelState.IsValid)
            {
                db.Congregations.Add(congregation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(congregation);
        }

        // GET: /Congregation/Edit/5
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Congregation congregation = db.Congregations.Find(id);
            if (congregation == null)
            {
                return HttpNotFound();
            }
            return View(congregation);
        }

        // POST: /Congregation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Edit([Bind(Include = "Id,Description")] Congregation congregation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(congregation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(congregation);
        }

        // GET: /Congregation/Delete/5
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Congregation congregation = db.Congregations.Find(id);
            if (congregation == null)
            {
                return HttpNotFound();
            }
            return View(congregation);
        }

        // POST: /Congregation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthLog(Roles = "SuperAdmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Congregation congregation = db.Congregations.Find(id);
            db.Congregations.Remove(congregation);
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
