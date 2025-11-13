using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Controllers
{
    public class ProfileController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // Đã BỎ hàm GetCurrentUserId() và các kiểm tra (if ... == null)

        // GET: Profile/Khachhang
        public ActionResult Khachhang()
        {
            // Lấy ID trực tiếp. 
            // Giả định rằng Session["id"] luôn tồn tại vì menu đã được lọc
            int loggedInKhachHangId = (int)Session["id"];

            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(loggedInKhachHangId);

            // Vẫn nên giữ kiểm tra này phòng trường hợp ID không tìm thấy trong CSDL
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }

            return View(kHACHHANG);
        }

        // GET: Profile/Edit
        public ActionResult Edit()
        {
            int loggedInKhachHangId = (int)Session["id"];
            KHACHHANG kHACHHANG = db.KHACHHANGs.Find(loggedInKhachHangId);

            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }

            return View(kHACHHANG);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TenKH,DiaChi,SDT,GioTinh,NgaySinh,Email,MK")] KHACHHANG formData)
        {
            // Lấy ID trực tiếp từ Session
            int loggedInKhachHangId = (int)Session["id"];

            // Kiểm tra các trường được bind
            if (ModelState.IsValidField("TenKH") && ModelState.IsValidField("DiaChi") &&
                ModelState.IsValidField("SDT") && ModelState.IsValidField("GioTinh") &&
                ModelState.IsValidField("NgaySinh") && ModelState.IsValidField("Email") &&
                ModelState.IsValidField("MK"))
            {
                KHACHHANG kHACHHANG_db = db.KHACHHANGs.Find(loggedInKhachHangId);

                if (kHACHHANG_db == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật thông tin
                kHACHHANG_db.TenKH = formData.TenKH;
                kHACHHANG_db.DiaChi = formData.DiaChi;
                kHACHHANG_db.SDT = formData.SDT;
                kHACHHANG_db.GioTinh = formData.GioTinh;
                kHACHHANG_db.NgaySinh = formData.NgaySinh;
                kHACHHANG_db.Email = formData.Email;

                if (!string.IsNullOrEmpty(formData.MK))
                {
                    kHACHHANG_db.MK = formData.MK; // (Vẫn khuyến nghị mã hóa mật khẩu này)
                }

                db.Entry(kHACHHANG_db).State = EntityState.Modified;
                db.SaveChanges();

                // Chuyển hướng về trang thông tin cá nhân
                return RedirectToAction("Khachhang");
            }

            // Nếu ModelState không hợp lệ, trả về form Edit
            return View(formData);
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