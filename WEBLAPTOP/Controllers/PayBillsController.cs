using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity;
using WEBLAPTOP.ViewModel;
using System.Threading.Tasks;
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

            List<KHUYENMAI> khuyenMai = db.KHUYENMAIs.Where(km=>km.TrangThai==1).ToList();
            ViewBag.KhuyenMai = khuyenMai;
            ViewBag.TenKH = khachHang.TenKH;
            ViewBag.DiaChi = khachHang.DiaChi;
            ViewBag.SDT = khachHang.SDT;
            ViewBag.TongTienHang = tongTienHang;
            return View(spGioHang);
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
            var tongTienHang=spGioHang.TongTien ?? 0;
            var ds_temp = new List<GioHangView>();
            ds_temp.Add(spGioHang);

            List<KHUYENMAI> khuyenMai = db.KHUYENMAIs.Where(km => km.TrangThai == 1).ToList();
            ViewBag.KhuyenMai = khuyenMai;
            ViewBag.TenKH = khachHang.TenKH;
            ViewBag.DiaChi = khachHang.DiaChi;
            ViewBag.SDT = khachHang.SDT;
            ViewBag.TongTienHang = tongTienHang;
            return View("Index",ds_temp);
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
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}