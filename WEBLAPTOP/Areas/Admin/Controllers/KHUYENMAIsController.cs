using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Cần thiết cho .ToListAsync() và .FindAsync()
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Cần thiết cho Task
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity.Infrastructure;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class KHUYENMAIsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/KHUYENMAIs
        public async Task<ActionResult> Index()
        {
            // Chuyển sang ToListAsync()
            return View(await db.KHUYENMAIs.ToListAsync());
        }

        // GET: Admin/KHUYENMAIs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Chuyển sang FindAsync()
            KHUYENMAI kHUYENMAI = await db.KHUYENMAIs.FindAsync(id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_KM,GiamGia,Mota")] KHUYENMAI kHUYENMAI)
        {
            if (ModelState.IsValid)
            {
                db.KHUYENMAIs.Add(kHUYENMAI);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(kHUYENMAI);
        }

        // GET: Admin/KHUYENMAIs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Chuyển sang FindAsync()
            KHUYENMAI kHUYENMAI = await db.KHUYENMAIs.FindAsync(id);
            if (kHUYENMAI == null)
            {
                return HttpNotFound();
            }
            return View(kHUYENMAI);
        }

        // POST: Admin/KHUYENMAIs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_KM,GiamGia,Mota")] KHUYENMAI kHUYENMAI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kHUYENMAI).State = EntityState.Modified;
                // Chuyển sang SaveChangesAsync()
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(kHUYENMAI);
        }

        // GET: Admin/KHUYENMAIs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var kHUYENMAI = await db.KHUYENMAIs.FindAsync(id);
                if (kHUYENMAI == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khuyến mãi." });
                }

                db.KHUYENMAIs.Remove(kHUYENMAI);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateException) // Bắt lỗi do khóa ngoại
            {
                // Trả về success = false, AJAX sẽ nhảy vào khối 'else'
                return Json(new { success = false, message = "Không thể xóa! Khuyến mãi này đang được đơn hàng sử dụng!" });
            }
            catch (Exception ex) // Bắt các lỗi chung khác
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
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