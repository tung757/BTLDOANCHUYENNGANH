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
using WEBLAPTOP.App_Start;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
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
        [HttpPost] // <-- Bắt buộc
        public async Task<ActionResult> ToggleTrangThai(int id)
        {
            try
            {
                var kHUYENMAI = await db.KHUYENMAIs.FindAsync(id);
                if (kHUYENMAI == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy mã." });
                }

                // Logic đảo ngược trạng thái:
                // Nếu đang là 1 (Đang chạy) -> đổi thành 0 (Đã tắt)
                // Nếu đang là 0 (hoặc khác 1) -> đổi thành 1 (Đang chạy)
                kHUYENMAI.TrangThai = (kHUYENMAI.TrangThai == 1) ? 0 : 1;

                await db.SaveChangesAsync();

                // Trả về trạng thái mới để JS cập nhật giao diện
                return Json(new { success = true, newStatus = kHUYENMAI.TrangThai });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
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