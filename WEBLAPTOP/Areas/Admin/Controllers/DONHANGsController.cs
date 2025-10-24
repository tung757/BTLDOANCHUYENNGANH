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
    public class DONHANGsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DONHANGs
        public ActionResult Index()
        {
            var dONHANGs = db.DONHANGs.Include(d => d.KHACHHANG).Include(d => d.KHUYENMAI);
            return View(dONHANGs.ToList());
        }

        // GET: Admin/DONHANGs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONHANG dONHANG = db.DONHANGs.Find(id);
            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            return View(dONHANG);
        }

        // GET: Admin/DONHANGs/Create
        public ActionResult Create()
        {
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH");
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota");
            return View();
        }

        // POST: Admin/DONHANGs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DH,NgayLap,GhiChu,TrangThai,ID_KH,ID_KM,Ten,DiaChiGiaoHang,SDT,PhuongthucTT")] DONHANG dONHANG)
        {
            if (ModelState.IsValid)
            {
                db.DONHANGs.Add(dONHANG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // GET: Admin/DONHANGs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONHANG dONHANG = db.DONHANGs.Find(id);
            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // POST: Admin/DONHANGs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DH,NgayLap,GhiChu,TrangThai,ID_KH,ID_KM,Ten,DiaChiGiaoHang,SDT,PhuongthucTT")] DONHANG dONHANG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dONHANG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // GET: Admin/DONHANGs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONHANG dONHANG = db.DONHANGs.Find(id);
            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            return View(dONHANG);
        }

        // POST: Admin/DONHANGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DONHANG dONHANG = db.DONHANGs.Find(id);
            db.DONHANGs.Remove(dONHANG);
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
