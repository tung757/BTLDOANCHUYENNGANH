using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Cần thiết cho .ToListAsync(), .FirstOrDefaultAsync()
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Cần thiết cho Task
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class DONHANGsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DONHANGs
        public async Task<ActionResult> Index()
        {
            var dONHANGs = db.DONHANGs.Include(d => d.KHACHHANG).Include(d => d.KHUYENMAI);
            return View(await dONHANGs.ToListAsync());
        }

        // GET: Admin/DONHANGs/Details/5
        // GET: Admin/DONHANGs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DONHANG dONHANG = await db.DONHANGs
                .Include(d => d.KHACHHANG)
                .Include(d => d.KHUYENMAI)
                // SỬA LỖI Ở ĐÂY: Dùng "DONHANG_SANPHAM" (không có "s")
                .Include(d => d.DONHANG_SANPHAM.Select(dsp => dsp.SANPHAM))
                .FirstOrDefaultAsync(d => d.ID_DH == id);

            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            return View(dONHANG);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTrangThai(int id, string trangThai) 
        {
            try
            {
                var allowedStatus = new List<string> { "Đang xác nhận", "Đang giao", "Đã giao", "Đã huỷ" };
                if (string.IsNullOrEmpty(trangThai) || !allowedStatus.Contains(trangThai))
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ." });
                }

                var dONHANG = await db.DONHANGs.FindAsync(id); 

                if (dONHANG == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                dONHANG.TrangThai = trangThai;
                await db.SaveChangesAsync(); 

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: Admin/DONHANGs/Edit/5
        public async Task<ActionResult> Edit(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONHANG dONHANG = await db.DONHANGs.FindAsync(id); 
            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // POST: Admin/DONHANGs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_DH,NgayLap,GhiChu,TrangThai,ID_KH,ID_KM,Ten,DiaChiGiaoHang,SDT,PhuongthucTT,PhuongThucNhanHang")] DONHANG dONHANG) // 
        {
            if (ModelState.IsValid)
            {
                db.Entry(dONHANG).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // GET: Admin/DONHANGs/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var dONHANG = await db.DONHANGs.FindAsync(id);
            if (dONHANG == null)
                return HttpNotFound();

            db.DONHANGs.Remove(dONHANG);
            await db.SaveChangesAsync();

            return Json(new { success = true });
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