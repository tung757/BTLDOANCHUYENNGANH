using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity;
using WEBLAPTOP.ViewModel;
namespace WEBLAPTOP.Controllers
{
    public class PayBillsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        public ActionResult Index()
        {
            string username = Session["username"] as string;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "login");

            var khachHang = db.KHACHHANGs.FirstOrDefault(kh => kh.TK == username);
            if (khachHang == null)
                return View("Error");

            var gioHang = db.GIOHANGs.FirstOrDefault(gh => gh.ID_KH == khachHang.ID_KH);
            if (gioHang == null)
            {
                // Tạo giỏ hàng mới cho khách
                gioHang = new GIOHANG
                {
                    ID_KH = khachHang.ID_KH,
                };
                db.GIOHANGs.Add(gioHang);
                db.SaveChanges();
            }
            // Lấy danh sách sản phẩm trong giỏ
            var spGioHang = db.GIOHANG_SANPHAM
                            .Include(x => x.SANPHAM)
                            .Where(x => x.ID_GH == gioHang.ID_GH)
                            .Select(x => new GioHangView
                            {
                                ID_SP = x.SANPHAM.ID_SP,
                                Images_url = x.SANPHAM.Images_url,
                                TenSP = x.SANPHAM.TenSP,
                                GiaBan = x.SANPHAM.GiaBan,
                                SoLuong = x.SoLuong,
                                TongTien = x.SoLuong * x.SANPHAM.GiaBan
                            }).ToList();
            var tongTienHang = spGioHang.Sum(item => item.TongTien ?? 0);

            //Khuyến mại

            List<KHUYENMAI> khuyenMai = db.KHUYENMAIs.ToList();
            ViewBag.KhuyenMai = khuyenMai;
            ViewBag.TenKH = khachHang.TenKH;
            ViewBag.DiaChi = khachHang.DiaChi;
            ViewBag.SDT = khachHang.SDT;
            ViewBag.TongTienHang = tongTienHang;
            return View(spGioHang);
        }

    }

    }