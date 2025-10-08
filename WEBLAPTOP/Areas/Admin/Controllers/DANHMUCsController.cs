using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class DANHMUCsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DANHMUCs
        public ActionResult Index()
        {
            return View(db.DANHMUCs.ToList());
        }

        // GET: Admin/DANHMUCs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHMUC dANHMUC = db.DANHMUCs.Find(id);
            if (dANHMUC == null)
            {
                return HttpNotFound();
            }
            return View(dANHMUC);
        }

        // GET: Admin/DANHMUCs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/DANHMUCs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.DANHMUCs.Add(dANHMUC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dANHMUC);
        }

        // GET: Admin/DANHMUCs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHMUC dANHMUC = db.DANHMUCs.Find(id);
            if (dANHMUC == null)
            {
                return HttpNotFound();
            }
            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dANHMUC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dANHMUC);
        }

        // GET: Admin/DANHMUCs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DANHMUC dANHMUC = db.DANHMUCs.Find(id);
            if (dANHMUC == null)
            {
                return HttpNotFound();
            }
            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DANHMUC dANHMUC = db.DANHMUCs.Find(id);
            db.DANHMUCs.Remove(dANHMUC);
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
