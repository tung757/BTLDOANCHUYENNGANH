using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DANHMUCsController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DANHMUCs
        public async Task<ActionResult> Index()
        {
            var danhMucs = await db.DANHMUCs.ToListAsync();
            return View(danhMucs);
        }

        // GET: Admin/DANHMUCs/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.DanhMucList = await db.DANHMUCs.ToListAsync();
            return View();
        }

        // POST: Admin/DANHMUCs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.DANHMUCs.Add(dANHMUC);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucList = await db.DANHMUCs.ToListAsync();
            return View(dANHMUC);
        }

        // GET: Admin/DANHMUCs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dANHMUC = await db.DANHMUCs.FindAsync(id);
            if (dANHMUC == null)
                return HttpNotFound();

            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dANHMUC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var dANHMUC = await db.DANHMUCs.FindAsync(id);
                if (dANHMUC == null)
                {
                    // Trả về JSON để AJAX xử lý, thay vì HttpNotFound
                    return Json(new { success = false, message = "Không tìm thấy danh mục để xóa." });
                }

                db.DANHMUCs.Remove(dANHMUC);
                await db.SaveChangesAsync();

                // Xóa thành công
                return Json(new { success = true });
            }
            catch (DbUpdateException) // Bắt lỗi do khóa ngoại (DbUpdateConcurrencyException là lỗi khác)
            {
                // Lỗi này xảy ra khi DANHMUC vẫn còn SANPHAMs
                return Json(new { success = false, message = "Không thể xóa! Danh mục này vẫn còn sản phẩm." });
            }
            catch (Exception ex) // Bắt các lỗi chung khác
            {
                // Ghi log lỗi ex.Message ở đây nếu cần
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
