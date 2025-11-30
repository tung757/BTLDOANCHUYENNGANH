
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
    }
}
