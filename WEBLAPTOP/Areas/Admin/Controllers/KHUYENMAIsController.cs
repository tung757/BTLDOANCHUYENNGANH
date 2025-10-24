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
    public class KHUYENMAIsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/KHUYENMAIs
        public ActionResult Index()
        {
            return View(db.KHUYENMAIs.ToList());
        }

        // GET: Admin/KHUYENMAIs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHUYENMAI kHUYENMAI = db.KHUYENMAIs.Find(id);
            if (kHUYENMAI == null)
            {
                return HttpNotFound();
            }
            return View(kHUYENMAI);
        }

        // GET: Admin/KHUYENMAIs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/KHUYENMAIs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_KM,GiamGia,Mota")] KHUYENMAI kHUYENMAI)
        {
            if (ModelState.IsValid)
            {
                db.KHUYENMAIs.Add(kHUYENMAI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kHUYENMAI);
        }

        // GET: Admin/KHUYENMAIs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHUYENMAI kHUYENMAI = db.KHUYENMAIs.Find(id);
            if (kHUYENMAI == null)
            {
                return HttpNotFound();
            }
            return View(kHUYENMAI);
        }

        // POST: Admin/KHUYENMAIs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_KM,GiamGia,Mota")] KHUYENMAI kHUYENMAI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kHUYENMAI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kHUYENMAI);
        }

        // GET: Admin/KHUYENMAIs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KHUYENMAI kHUYENMAI = db.KHUYENMAIs.Find(id);
            if (kHUYENMAI == null)
            {
                return HttpNotFound();
            }
            return View(kHUYENMAI);
        }

        // POST: Admin/KHUYENMAIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KHUYENMAI kHUYENMAI = db.KHUYENMAIs.Find(id);
            db.KHUYENMAIs.Remove(kHUYENMAI);
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
