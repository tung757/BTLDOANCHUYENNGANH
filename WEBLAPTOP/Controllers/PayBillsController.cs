using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity;
using WEBLAPTOP.ViewModel;
using System.Threading.Tasks;
using System.Configuration;
using WEBLAPTOP.VNPAY;
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
                                TongTien = (decimal)x.SoLuong * x.SANPHAM.GiaBan
                            }).ToList();
            var tongTienHang = spGioHang.Sum(item => item.TongTien ?? 0);

            //Khuyến mại

            List<KHUYENMAI> khuyenMai = db.KHUYENMAIs.Where(km => km.TrangThai == 1).ToList();
            ViewBag.KhuyenMai = khuyenMai;
            ViewBag.TenKH = khachHang.TenKH;
            ViewBag.DiaChi = khachHang.DiaChi;
            ViewBag.SDT = khachHang.SDT;
            ViewBag.TongTienHang = tongTienHang;
            return View(spGioHang);
        }
        public ActionResult ThanhToanThanhCong()
        {
            return View();
        }

        public ActionResult ThanhToanThatBai()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> QuickBuy(int id_sp, int so_luong = 1)
        {
            string username = Session["username"] as string;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Index", "login");

            var khachHang = await db.KHACHHANGs.FirstOrDefaultAsync(kh => kh.TK == username);
            if (khachHang == null)
                return View("Error");
            var sp = db.SANPHAMs.FirstOrDefault(s => s.ID_SP == id_sp);
            var spGioHang = new GioHangView
            {
                ID_SP = sp.ID_SP,
                Images_url = sp.Images_url,
                TenSP = sp.TenSP,
                GiaBan = sp.GiaBan,
                SoLuong = so_luong,
                TongTien = so_luong * sp.GiaBan
            };
            var tongTienHang = spGioHang.TongTien ?? 0;
            var ds_temp = new List<GioHangView>();
            ds_temp.Add(spGioHang);

            List<KHUYENMAI> khuyenMai = db.KHUYENMAIs.Where(km => km.TrangThai == 1).ToList();
            ViewBag.KhuyenMai = khuyenMai;
            ViewBag.TenKH = khachHang.TenKH;
            ViewBag.DiaChi = khachHang.DiaChi;
            ViewBag.SDT = khachHang.SDT;
            ViewBag.TongTienHang = tongTienHang;
            return View("Index", ds_temp);
        }
        private void XoaGioHang(int? idKhachHang)
        {
            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.ID_KH == idKhachHang);
            if (gioHang == null) return;

            var dsSanPham = db.GIOHANG_SANPHAM
                              .Where(x => x.ID_GH == gioHang.ID_GH)
                              .ToList();

            if (dsSanPham.Any())
            {
                db.GIOHANG_SANPHAM.RemoveRange(dsSanPham);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public JsonResult DatHang(DonHangView model)
        {
            try
            {
                string username = Session["username"] as string;
                if (string.IsNullOrEmpty(username))
                    return Json(new { success = false, message = "Bạn chưa đăng nhập!" });

                var khachHang = db.KHACHHANGs.FirstOrDefault(kh => kh.TK == username);
                var dh = model.DONHANG;

                // 1. Khai báo và khởi tạo biến donHang trước
                var donHang = new DONHANG
                {
                    NgayLap = DateTime.Now,
                    GhiChu = dh.GhiChu,
                    TrangThai = "Chờ xác nhận",
                    ID_KH = khachHang.ID_KH,
                    ID_KM = dh.ID_KM,
                    Ten = dh.Ten,
                    DiaChiGiaoHang = dh.DiaChiGiaoHang,
                    SDT = dh.SDT,
                    PhuongthucTT = dh.PhuongthucTT,
                    PhuongThucNhanHang = dh.PhuongThucNhanHang
                };

                db.DONHANGs.Add(donHang);
                db.SaveChanges(); // Lưu để sinh ra ID_DH tự động

                // 2. Bây giờ donHang đã tồn tại trong context, có thể sử dụng ID_DH
                foreach (var sp in model.DONHANG_SANPHAM)
                {
                    // Kiểm tra tồn kho trước khi trừ
                    var sanPhamGoc = db.SANPHAMs.Find(sp.ID_SP);
                    if (sanPhamGoc == null || sanPhamGoc.SoLuong < sp.SoLuong)
                    {
                        return Json(new { success = false, message = "Sản phẩm " + sanPhamGoc?.TenSP + " không đủ hàng!" });
                    }

                    // Thêm chi tiết đơn hàng
                    var chiTiet = new DONHANG_SANPHAM
                    {
                        ID_DH = donHang.ID_DH, // Đã có thể truy cập biến donHang ở đây
                        ID_SP = sp.ID_SP,
                        SoLuong = sp.SoLuong,
                        DonGia = sp.DonGia
                    };
                    db.DONHANG_SANPHAM.Add(chiTiet);

                    // 3. Thực hiện trừ số lượng tồn kho và tăng số lượng bán
                    sanPhamGoc.SoLuong -= sp.SoLuong;
                    sanPhamGoc.SoLuongBan = (sanPhamGoc.SoLuongBan ?? 0) + sp.SoLuong;
                }

                db.SaveChanges();
                // ... (Logic xóa giỏ hàng)
                XoaGioHang(khachHang.ID_KH);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateVnpayPayment(DonHangView model)
        {
            string username = Session["username"] as string;
            var kh = db.KHACHHANGs.First(x => x.TK == username);

            // 1. Lưu đơn hàng CHỜ THANH TOÁN
            var donHang = new DONHANG
            {
                NgayLap = DateTime.Now,
                TrangThai = "Chờ thanh toán",
                ID_KH = kh.ID_KH,
                ID_KM = model.DONHANG.ID_KM,
                Ten = model.DONHANG.Ten,
                SDT = model.DONHANG.SDT,
                DiaChiGiaoHang = model.DONHANG.DiaChiGiaoHang,
                PhuongthucTT = "VNPAY",
                PhuongThucNhanHang = model.DONHANG.PhuongThucNhanHang,
                GhiChu = model.DONHANG.GhiChu
            };
            db.DONHANGs.Add(donHang);
            db.SaveChanges();

            foreach (var sp in model.DONHANG_SANPHAM)
            {
                // Kiểm tra tồn kho trước khi trừ
                var sanPhamGoc = db.SANPHAMs.Find(sp.ID_SP);
                if (sanPhamGoc == null || sanPhamGoc.SoLuong < sp.SoLuong)
                {
                    return Json(new { success = false, message = "Sản phẩm " + sanPhamGoc?.TenSP + " không đủ hàng!" });
                }

                // Thêm chi tiết đơn hàng
                var chiTiet = new DONHANG_SANPHAM
                {
                    ID_DH = donHang.ID_DH, 
                    ID_SP = sp.ID_SP,
                    SoLuong = sp.SoLuong,
                    DonGia = sp.DonGia
                };
                db.DONHANG_SANPHAM.Add(chiTiet);
            }
            db.SaveChanges();

            long tongTienHang = model.DONHANG_SANPHAM.Sum(x => (long)(x.SoLuong * (x.DonGia ?? 0)));

            long tienGiam = 0;

            if (model.DONHANG.ID_KM != null)
            {
                var km = db.KHUYENMAIs.Find(model.DONHANG.ID_KM);
                if (km != null)
                {
                    tienGiam = (long)(tongTienHang * (km.GiamGia / 100.0));
                }

            }
            long amount = (tongTienHang - tienGiam)*100;

            var vnpay = new VnpayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["vnp_TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {donHang.ID_DH}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", ConfigurationManager.AppSettings["vnp_ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", donHang.ID_DH.ToString());

            string url = vnpay.CreateRequestUrl(
                ConfigurationManager.AppSettings["vnp_Url"],
                ConfigurationManager.AppSettings["vnp_HashSecret"]
            );

            return Json(new { success = true, url = url });
        }
        public ActionResult VnpayReturn()
        {
            var vnpay = new VnpayLibrary();
            foreach (string key in Request.QueryString)
            {
                if (!string.IsNullOrEmpty(Request.QueryString[key]))
                    vnpay.AddResponseData(key, Request.QueryString[key]);
            }

            bool isValid = vnpay.ValidateSignature(
                Request.QueryString["vnp_SecureHash"],
                ConfigurationManager.AppSettings["vnp_HashSecret"]
            );

            if (!isValid)
                return View("ThanhToanThatBai");

            string responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            int orderId = int.Parse(vnpay.GetResponseData("vnp_TxnRef"));

            var donHang = db.DONHANGs
                .Include(d => d.DONHANG_SANPHAM)
                .FirstOrDefault(d => d.ID_DH == orderId);

            if (donHang == null)
                return View("ThanhToanThatBai");

            if (responseCode == "00")
            {
                donHang.TrangThai = "Đã thanh toán";

                foreach (var ct in donHang.DONHANG_SANPHAM)
                {
                    var sp = db.SANPHAMs.Find(ct.ID_SP);
                    if (sp == null || sp.SoLuong < ct.SoLuong)
                        return View("ThanhToanThatBai");

                    sp.SoLuong -= ct.SoLuong;
                    sp.SoLuongBan = (sp.SoLuongBan ?? 0) + ct.SoLuong;
                }
                XoaGioHang(donHang.ID_KH);
                db.SaveChanges();
                return View("ThanhToanThanhCong");
            }
            else
            {
                donHang.TrangThai = "Thanh toán thất bại";
                db.SaveChanges();
                return View("ThanhToanThatBai");
            }
        }
    }
}