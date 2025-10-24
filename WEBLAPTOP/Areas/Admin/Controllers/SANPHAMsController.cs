using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Cần cho .ToListAsync(), .FindAsync(), ...
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Cần cho Task<>
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class SANPHAMsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/SANPHAMs
        public async Task<ActionResult> Index()
        {
            var sANPHAMs = db.SANPHAMs.Include(s => s.DANHMUC);
            return View(await sANPHAMs.ToListAsync());
        }

        // GET: Admin/SANPHAMs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // ĐÃ SỬA: Dùng FindAsync
            SANPHAM sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,ID_DM")] SANPHAM sANPHAM, HttpPostedFileBase ImagesFile)
        {
            if (ModelState.IsValid)
            {
                if (ImagesFile != null && ImagesFile.ContentLength > 0)
                {

                    string productName = sANPHAM.TenSP ?? "san-pham";

                    string invalidChars = new string(Path.GetInvalidFileNameChars());
                    string sanitizedName = new string(productName.Where(ch => !invalidChars.Contains(ch)).ToArray());
                    sanitizedName = sanitizedName.Replace(" ", "-").ToLower(); 

                    string extension = Path.GetExtension(ImagesFile.FileName);

                    string fileName = $"{sanitizedName}-{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";


                    string folderPath = Server.MapPath("~/Images/Product_images/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, fileName);

                    ImagesFile.SaveAs(filePath);

                    sANPHAM.Images_url = "/Images/Product_images/" + fileName;
                }
                else
                {
                    sANPHAM.Images_url = "/Images/Product_images/default.jpg";
                }

                sANPHAM.NgayTao = DateTime.Now;
                db.SANPHAMs.Add(sANPHAM);

                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Nếu ModelState sai → render lại View
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            // ĐÃ SỬA: Dùng ToListAsync()
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // POST: Admin/SANPHAMs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,ID_DM")] SANPHAM sANPHAM, HttpPostedFileBase ImagesFile)
        {
            if (ModelState.IsValid)
            {

                // 1. Lấy đường dẫn ảnh cũ từ DB
                string oldImagePath = null;
                var existingProduct = await db.SANPHAMs.AsNoTracking().FirstOrDefaultAsync(p => p.ID_SP == sANPHAM.ID_SP);
                if (existingProduct != null)
                {
                    oldImagePath = existingProduct.Images_url;
                }

                // 2. Kiểm tra xem có file ảnh MỚI được tải lên không
                if (ImagesFile != null && ImagesFile.ContentLength > 0)
                {

                    // 1. Lấy tên sản phẩm
                    string productName = sANPHAM.TenSP ?? "san-pham";

                    // 2. "Làm sạch" tên
                    string invalidChars = new string(Path.GetInvalidFileNameChars());
                    string sanitizedName = new string(productName.Where(ch => !invalidChars.Contains(ch)).ToArray());
                    sanitizedName = sanitizedName.Replace(" ", "-").ToLower();

                    // 3. Lấy phần mở rộng file gốc
                    string extension = Path.GetExtension(ImagesFile.FileName);

                    // 4. Tạo tên file duy nhất
                    string fileName = $"{sanitizedName}-{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";


                    // Tạo thư mục
                    string folderPath = Server.MapPath("~/Images/Product_images/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Đường dẫn tuyệt đối
                    string filePath = Path.Combine(folderPath, fileName);

                    // Lưu file mới
                    ImagesFile.SaveAs(filePath);

                    // Gán đường dẫn MỚI vào DB
                    sANPHAM.Images_url = "/Images/Product_images/" + fileName;

                    // Xóa file ảnh cũ (nếu khác file mới và không phải là file default)
                    if (!string.IsNullOrEmpty(oldImagePath) &&
                         oldImagePath != sANPHAM.Images_url &&
                         !oldImagePath.EndsWith("default.jpg")) // Không xóa file default
                    {
                        string oldAbsoluteFile = Server.MapPath(oldImagePath);
                        if (System.IO.File.Exists(oldAbsoluteFile))
                        {
                            try
                            {
                                System.IO.File.Delete(oldAbsoluteFile);
                            }
                            catch (Exception) { /* Bỏ qua nếu không xóa được */ }
                        }
                    }
                }
                else
                {
                    // Nếu không tải file mới, giữ lại đường dẫn ảnh cũ
                    sANPHAM.Images_url = oldImagePath;
                }

                // Cập nhật thông tin sản phẩm vào DB
                db.Entry(sANPHAM).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            // Nếu ModelState sai
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
                return HttpNotFound();

            // Ghi chú: Bạn có thể thêm logic xóa file ảnh ở đây
            if (!string.IsNullOrEmpty(sANPHAM.Images_url))
            {
                string absolutePath = Server.MapPath(sANPHAM.Images_url);
                if (System.IO.File.Exists(absolutePath))
                {
                    try { System.IO.File.Delete(absolutePath); }
                    catch (Exception) { /* Bỏ qua */ }
                }
            }

            db.SANPHAMs.Remove(sANPHAM);
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