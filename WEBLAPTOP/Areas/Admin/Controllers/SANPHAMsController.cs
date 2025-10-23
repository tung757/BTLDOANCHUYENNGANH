using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class SANPHAMsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/SANPHAMs
        public ActionResult Index()
        {
            var sANPHAMs = db.SANPHAMs.Include(s => s.DANHMUC);
            return View(sANPHAMs.ToList());
        }
        // GET: Admin/SANPHAMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = db.SANPHAMs.Find(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Create
        public ActionResult Create()
        {
            ViewBag.ID_DM = new SelectList(db.DANHMUCs, "ID_DM", "TenDM");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,ID_DM")] SANPHAM sANPHAM, HttpPostedFileBase ImagesFile)
        {
            if (ModelState.IsValid)
            {
                // --- Kiểm tra và lưu file ảnh ---
                if (ImagesFile != null && ImagesFile.ContentLength > 0)
                {
                    // Lấy tên file gốc
                    string fileName = Path.GetFileName(ImagesFile.FileName);

                    // Tạo thư mục lưu ảnh nếu chưa tồn tại
                    string folderPath = Server.MapPath("~/Images/SanPham/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Đường dẫn tuyệt đối trên server
                    string filePath = Path.Combine(folderPath, fileName);

                    // Lưu file
                    ImagesFile.SaveAs(filePath);

                    // Gán đường dẫn tương đối vào DB
                    sANPHAM.Images_url = "/Images/SanPham/" + fileName;
                }
                else
                {
                    // Nếu không chọn ảnh, gán ảnh mặc định (tùy bạn)
                    sANPHAM.Images_url = "/Images/SanPham/default.jpg";
                }

                // --- Lưu thông tin sản phẩm ---
                sANPHAM.NgayTao = DateTime.Now;
                db.SANPHAMs.Add(sANPHAM);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu ModelState sai → render lại View
            ViewBag.ID_DM = new SelectList(db.DANHMUCs, "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }



        // GET: Admin/SANPHAMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = db.SANPHAMs.Find(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DM = new SelectList(db.DANHMUCs, "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // POST: Admin/SANPHAMs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,Images_url,ID_DM")] SANPHAM sANPHAM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sANPHAM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_DM = new SelectList(db.DANHMUCs, "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
                return HttpNotFound();

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
