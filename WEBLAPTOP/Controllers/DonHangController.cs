using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using WEBLAPTOP.ViewModel;

namespace WEBLAPTOP.Controllers
{
    public class DonHangController : Controller
    {
        // GET: DonHang
        public ActionResult Index()
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var dsDonHang = db.DONHANGs
                              .Where(dh => dh.ID_KH == id_kh)
                              .OrderByDescending(dh => dh.NgayLap)
                              .ToList();

            return View(dsDonHang);
        }
        public ActionResult Details(int id)
        {
            int? id_kh = Session["id"] as int?;
            if (id_kh == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var donHang = db.DONHANGs
                            .Include("DONHANG_SANPHAM.SANPHAM")
                            .FirstOrDefault(dh => dh.ID_DH == id && dh.ID_KH == id_kh);

            if (donHang == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }

            return View(donHang);
        }
    }
}