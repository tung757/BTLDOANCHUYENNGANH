//using System.Collections.Generic;
//using System;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using WEBLAPTOP.Models;
//using WEBLAPTOP.ViewModel;

//namespace WEBLAPTOP.Controllers
//{
//    public class DonHangController : Controller
//    {
//        // GET: DonHang
//        private readonly DARKTHESTORE db = new DARKTHESTORE();
//        public ActionResult Checkout()
//        {
//            int? id_kh = Session["id"] as int?;
//            if (id_kh == null)
//            {
//                return RedirectToAction("Index", "Login");
//            }
//            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh);
//            var cart = new List<CartItem>();
//            if (gioHang != null)
//            {
//                var cartItemsQuery = from gh_sp in db.GIOHANG_SANPHAM
//                                     join sp in db.SANPHAMs on gh_sp.ID_SP equals sp.ID_SP
//                                     where gh_sp.ID_GH == gioHang.ID_GH
//                                     select new WEBLAPTOP.ViewModel.CartItem
//                                     {
//                                         ID_SP = sp.ID_SP,
//                                         TenSP = sp.TenSP,
//                                         Images_url = sp.Images_url,
//                                         GiaBan = sp.GiaBan,
//                                         SoLuong = gh_sp.SoLuong,
//                                         TongTien = gh_sp.SoLuong * sp.GiaBan
//                                     };
//                cart = cartItemsQuery.ToList();
//            }

//            if (cart.Count == 0)
//            {
//                return RedirectToAction("Index", "Cart");
//            }
//            var khachHang = db.KHACHHANGs.Find(id_kh);
//            ViewBag.KhachHang = khachHang;
//            ViewBag.KhuyenMai = db.KHUYENMAIs.Where(km => km.TrangThai == 1).ToList();
//            ViewBag.CartItems = cart;
//            ViewBag.TotalQuantity = cart.Sum(item => item.SoLuong.GetValueOrDefault(0));
//            ViewBag.TotalPrice = cart.Sum(item => item.TongTien.GetValueOrDefault(0));

//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult PlaceOrder(FormCollection form)
//        {
//            int? id_kh = Session["id"] as int?;
//            if (id_kh == null)
//            {
//                return RedirectToAction("Index", "Login");
//            }
//            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == id_kh);
//            if (gioHang == null)
//            {
//                return RedirectToAction("Index", "Cart");
//            }
//            var cartItems = db.GIOHANG_SANPHAM.Where(gh_sp => gh_sp.ID_GH == gioHang.ID_GH).ToList();

//            if (!cartItems.Any())
//            {
//                return RedirectToAction("Index", "Cart");
//            }
//            using (var transaction = db.Database.BeginTransaction())
//            {
//                try
//                {
//                    var donHang = new DONHANG();
//                    donHang.Ten = form["Ten"];
//                    donHang.SDT = form["SDT"];
//                    donHang.DiaChiGiaoHang = form["DiaChiGiaoHang"];
//                    donHang.PhuongthucTT = form["PhuongthucTT"];
//                    donHang.PhuongThucNhanHang = "Giao hàng tận nơi";
//                    donHang.GhiChu = form["GhiChu"];

//                    int id_km = 0;
//                    int.TryParse(form["ID_KM"], out id_km);
//                    donHang.ID_KM = id_km > 0 ? (int?)id_km : null;
//                    donHang.ID_KH = id_kh;
//                    donHang.NgayLap = DateTime.Now;
//                    donHang.TrangThai = "Chờ xác nhận";

//                    db.DONHANGs.Add(donHang);
//                    db.SaveChanges();

//                    int id_dh_moi = donHang.ID_DH;
//                    foreach (var item in cartItems)
//                    {
//                        var sanPham = db.SANPHAMs.Find(item.ID_SP);
//                        if (sanPham == null || sanPham.SoLuong < item.SoLuong)
//                        {
//                            throw new Exception("Sản phẩm " + (sanPham?.TenSP ?? "ID " + item.ID_SP) + " không đủ số lượng.");
//                        }

//                        var chiTietDonHang = new DONHANG_SANPHAM
//                        {
//                            ID_DH = id_dh_moi,
//                            ID_SP = item.ID_SP,
//                            SoLuong = item.SoLuong,
//                            DonGia = sanPham.GiaBan
//                        };
//                        db.DONHANG_SANPHAM.Add(chiTietDonHang);
//                        sanPham.SoLuong -= item.SoLuong;
//                        sanPham.SoLuongBan = (sanPham.SoLuongBan ?? 0) + item.SoLuong;
//                    }
//                    db.GIOHANG_SANPHAM.RemoveRange(cartItems);
//                    db.SaveChanges();
//                    transaction.Commit();

//                    return RedirectToAction("OrderSuccess");
//                }
//                catch (Exception ex)
//                {
//                    transaction.Rollback();
//                    TempData["CheckoutError"] = "Lỗi khi đặt hàng: " + ex.Message;
//                    return RedirectToAction("Checkout");
//                }
//            }
//        }
//        public ActionResult OrderSuccess()
//        {
//            ViewBag.Message = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
//            return View();
//        }
//        public ActionResult Index()
//        {
//            int? id_kh = Session["id"] as int?;
//            if (id_kh == null)
//            {
//                return RedirectToAction("Index", "Login");
//            }
//            var dsDonHang = db.DONHANGs
//                              .Where(dh => dh.ID_KH == id_kh)
//                              .OrderByDescending(dh => dh.NgayLap)
//                              .ToList();

//            return View(dsDonHang);
//        }
//        public ActionResult Details(int id)
//        {
//            int? id_kh = Session["id"] as int?;
//            if (id_kh == null)
//            {
//                return RedirectToAction("Index", "Login");
//            }
//            var donHang = db.DONHANGs
//                            .Include("DONHANG_SANPHAM.SANPHAM")
//                            .FirstOrDefault(dh => dh.ID_DH == id && dh.ID_KH == id_kh);

//            if (donHang == null)
//            {
//                return HttpNotFound("Không tìm thấy đơn hàng.");
//            }

//            return View(donHang);
//        }
//    }
//}





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

        //  DANH SÁCH ĐƠN HÀNG CỦA KHÁCH
       
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
