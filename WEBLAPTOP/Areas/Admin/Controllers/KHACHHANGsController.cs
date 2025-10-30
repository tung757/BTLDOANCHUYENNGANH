using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; 
using System.Linq;
using System.Net;
using System.Threading.Tasks; 
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity.Infrastructure; 

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class KHACHHANGsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/KHACHHANGs (Async)
        public async Task<ActionResult> Index() 
        {
            // Dùng ToListAsync()
            return View(await db.KHACHHANGs.ToListAsync());
        }

        // GET: Admin/KHACHHANGs/Details/5 (Async)
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Dùng FindAsync()
            KHACHHANG kHACHHANG = await db.KHACHHANGs.FindAsync(id); 
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }
        // GET: Admin/KHACHHANGs/Edit/5 (Async)
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Dùng FindAsync()
            KHACHHANG kHACHHANG = await db.KHACHHANGs.FindAsync(id); 
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }

        // POST: Admin/KHACHHANGs/Edit/5 (Async)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_KH,TenKH,DiaChi,SDT,GioTinh,NgaySinh,Email,TK,MK,PhanQuyen")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                // Xử lý Mật khẩu: Chỉ cập nhật nếu có nhập mới
                if (string.IsNullOrEmpty(kHACHHANG.MK))
                {
                    db.Entry(kHACHHANG).State = EntityState.Modified;
                    db.Entry(kHACHHANG).Property(x => x.MK).IsModified = false; // Không sửa MK
                }
                else
                {
                    // (!!! LƯU Ý BẢO MẬT: Nên mã hóa MK mới ở đây !!!)
                    db.Entry(kHACHHANG).State = EntityState.Modified; // Sửa bình thường (bao gồm MK)
                }

                await db.SaveChangesAsync(); 
                return RedirectToAction("Index");
            }
            return View(kHACHHANG);
        }

        // POST: Admin/KHACHHANGs/Delete/5 (Đã Async từ trước)
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var kHACHHANG = await db.KHACHHANGs.FindAsync(id);
                if (kHACHHANG == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                db.KHACHHANGs.Remove(kHACHHANG);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateException) // Lỗi khóa ngoại
            {
                return Json(new { success = false, message = "Không thể xóa! Khách hàng này đã có dữ liệu liên quan (đơn hàng, giỏ hàng, đánh giá)." });
            }
            catch (Exception ex) // Lỗi chung
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // Dispose (Không đổi)
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