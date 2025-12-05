
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity;

namespace WEBLAPTOP.Controllers
{
    public class DonHangController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        //  DANH SÁCH ĐƠN HÀNG CỦA KHÁCH OK

        public ActionResult Index()
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
                return RedirectToAction("Index", "Login");

            var dsDonHang = db.DONHANGs
                              .Where(dh => dh.ID_KH == id_kh)
                              .OrderByDescending(dh => dh.NgayLap)
                              .ToList();

            return View(dsDonHang);
        }

        //  CHI TIẾT 1 ĐƠN HÀNG
        public ActionResult Details(int id)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
                return RedirectToAction("Index", "Login");

            var donHang = db.DONHANGs
                            .Include(d => d.DONHANG_SANPHAM.Select(x => x.SANPHAM))
                            .FirstOrDefault(d => d.ID_DH == id && d.ID_KH == id_kh);

            if (donHang == null)
                return HttpNotFound("Không tìm thấy đơn hàng!");

            return View(donHang);
        }
        // HỦY ĐƠN HÀNG - Dành cho khách hàng
        [HttpPost]
        public ActionResult HuyDonHang(int id)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });

            var donHang = db.DONHANGs.FirstOrDefault(dh => dh.ID_DH == id && dh.ID_KH == id_kh);
            if (donHang == null)
                return Json(new { success = false, message = "Không tìm thấy đơn hàng hoặc đơn hàng không thuộc về bạn." });
            if (donHang.TrangThai != "Chờ xác nhận")
            {
                return Json(new { success = false, message = $"Chỉ có thể hủy đơn hàng ở trạng thái 'Chờ xác nhận'. Đơn hàng này đang ở trạng thái: {donHang.TrangThai}." });
            }
            try
            {
                donHang.TrangThai = "Hủy";
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
            }
            catch (Exception)
            { 
                return Json(new { success = false, message = "Lỗi trong quá trình xử lý hủy đơn hàng. Vui lòng thử lại." });
            }
        }
    }
}
