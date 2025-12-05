using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SystemSettingsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/SystemSettings/Index
        public ActionResult Index()
        {
            // Lấy danh sách QUANGCAO để hiển thị ở trang Admin
            var banners = db.QUANGCAOs.ToList();

            // Truyền banners vào ViewBag để View sử dụng
            ViewBag.Banners = banners;

            return View();
        }

        // POST: Upload Banner Mới
        [HttpPost]
        public ActionResult UploadBanner(IEnumerable<HttpPostedFileBase> bannerFiles)
        {
            // Chỉ xử lý nếu người dùng CÓ chọn file
            if (bannerFiles != null && bannerFiles.Count() > 0 && bannerFiles.First() != null)
            {
                string path = Server.MapPath("~/Images/Logo/");

                // 1. --- XÓA SẠCH BANNER CŨ (RESET) ---
                // Lấy tất cả banner cũ ra
                var bannerCu = db.QUANGCAOs.Where(q => q.Loai == "Banner").ToList();

                foreach (var item in bannerCu)
                {
                    // Xóa file ảnh cũ trên ổ cứng (Dọn rác server)
                    string fullPath = Path.Combine(path, item.Url_Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        try { System.IO.File.Delete(fullPath); } catch { }
                    }
                    // Xóa trong Database
                    db.QUANGCAOs.Remove(item);
                }
                db.SaveChanges(); // Lưu lệnh xóa
                                  // -------------------------------------

                // 2. --- THÊM BANNER MỚI ---
                foreach (var file in bannerFiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string extension = Path.GetExtension(file.FileName);
                            // Đặt tên file unique
                            string fileName = "banner-" + Guid.NewGuid().ToString().Substring(0, 8) + extension;

                            file.SaveAs(Path.Combine(path, fileName));

                            var qc = new QUANGCAO();
                            qc.Loai = "Banner";
                            qc.Url_Image = fileName;

                            db.QUANGCAOs.Add(qc);
                        }
                        catch { continue; }
                    }
                }
                db.SaveChanges(); // Lưu các banner mới
            }

            return RedirectToAction("Index");
        }
        // POST: Xóa Banner (bằng AJAX)
        [HttpPost]
        public ActionResult DeleteBanner(int id)
        {
            var qc = db.QUANGCAOs.Find(id);
            if (qc != null)
            {
                // Xóa file vật lý
                string fullPath = Server.MapPath("~/Images/Logo/" + qc.Url_Image);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                // Xóa trong DB
                db.QUANGCAOs.Remove(qc);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}