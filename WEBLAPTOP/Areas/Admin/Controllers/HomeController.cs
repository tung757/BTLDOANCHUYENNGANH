using System;
using System.Collections.Generic;
using System.Data.Entity; // Cần cho DbFunctions và các hàm ngày tháng
using System.Linq;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/Home
        public ActionResult Index()
        {
            var model = new ThongKe();

            // 1. Các thẻ thống kê (Giữ nguyên code cũ của bạn)
            model.TongKhachHang = db.KHACHHANGs.Count();
            model.TongSanPham = db.SANPHAMs.Count();
            model.TongDonHang = db.DONHANGs.Count();
            model.SanPhamSapHet = db.SANPHAMs.Count(s => s.SoLuong != null && s.SoLuong < 5);

            // 2. Lấy đơn hàng mới nhất (Giữ nguyên logic cũ)
            var recentOrders = db.DONHANGs.OrderByDescending(d => d.NgayLap).Take(5).ToList();
            model.DonHangMoiNhat = new List<DonHangHienThi>();
            foreach (var item in recentOrders)
            {
                decimal total = item.DONHANG_SANPHAM.Sum(x => (long)(x.SoLuong ?? 0) * (x.DonGia ?? 0));
                model.DonHangMoiNhat.Add(new DonHangHienThi
                {
                    ID_DH = item.ID_DH,
                    TenKhachHang = item.Ten,
                    TongTien = total,
                    TrangThai = item.TrangThai
                });
            }

            // 3. Khách hàng mới (Giữ nguyên logic cũ)
            model.KhachHangMoi = db.KHACHHANGs.OrderByDescending(k => k.ID_KH).Take(5).ToList();

            return View(model);
        }

        // API Lấy dữ liệu biểu đồ
        [HttpPost]
        public ActionResult GetRevenueData(string mode)
        {
            var labels = new List<string>();
            var data = new List<decimal>();
            DateTime now = DateTime.Now;

            // 1. Lấy danh sách đơn hàng thành công
            // (Lưu ý: Kéo .ToList() về bộ nhớ trước để tránh lỗi LINQ phức tạp với bảng con)
            var validOrders = db.DONHANGs
                .Where(d => d.TrangThai == "Đã giao" || d.TrangThai == "Đã hoàn thành")
                .Include(d => d.DONHANG_SANPHAM) // Tải kèm chi tiết để tính tiền
                .ToList();

            if (mode == "WEEK") // 7 ngày qua
            {
                for (int i = 6; i >= 0; i--)
                {
                    DateTime date = now.AddDays(-i).Date;
                    labels.Add(date.ToString("dd/MM"));

                    var orders = validOrders.Where(d => d.NgayLap.HasValue && d.NgayLap.Value.Date == date);
                    // Tính tổng tiền: Sum(SoLuong * DonGia)
                    decimal total = orders.Sum(d => d.DONHANG_SANPHAM.Sum(c => (long)(c.SoLuong ?? 0) * (c.DonGia ?? 0)));
                    data.Add(total);
                }
            }
            else if (mode == "MONTH") // Các ngày trong tháng hiện tại
            {
                int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    labels.Add(i.ToString());
                    // Chỉ lấy đơn của tháng này, năm này
                    var orders = validOrders.Where(d => d.NgayLap.HasValue && d.NgayLap.Value.Day == i && d.NgayLap.Value.Month == now.Month && d.NgayLap.Value.Year == now.Year);

                    decimal total = orders.Sum(d => d.DONHANG_SANPHAM.Sum(c => (long)(c.SoLuong ?? 0) * (c.DonGia ?? 0)));
                    data.Add(total);
                }
            }
            else if (mode == "YEAR") // 12 tháng trong năm nay
            {
                for (int i = 1; i <= 12; i++)
                {
                    labels.Add("Tháng " + i);
                    var orders = validOrders.Where(d => d.NgayLap.HasValue && d.NgayLap.Value.Month == i && d.NgayLap.Value.Year == now.Year);

                    decimal total = orders.Sum(d => d.DONHANG_SANPHAM.Sum(c => (long)(c.SoLuong ?? 0) * (c.DonGia ?? 0)));
                    data.Add(total);
                }
            }
            else if (mode == "ALL") // TOÀN BỘ THỜI GIAN (Gộp theo Tháng/Năm)
            {
                // Tìm ngày đơn hàng đầu tiên và cuối cùng
                if (validOrders.Any())
                {
                    var minDate = validOrders.Min(d => d.NgayLap) ?? now;
                    var maxDate = validOrders.Max(d => d.NgayLap) ?? now;

                    // Duyệt từng tháng từ lúc bắt đầu đến kết thúc
                    for (DateTime date = new DateTime(minDate.Year, minDate.Month, 1); date <= maxDate; date = date.AddMonths(1))
                    {
                        labels.Add(date.ToString("MM/yyyy")); // Nhãn là "10/2025"

                        var orders = validOrders.Where(d => d.NgayLap.HasValue && d.NgayLap.Value.Month == date.Month && d.NgayLap.Value.Year == date.Year);

                        decimal total = orders.Sum(d => d.DONHANG_SANPHAM.Sum(c => (long)(c.SoLuong ?? 0) * (c.DonGia ?? 0)));
                        data.Add(total);
                    }
                }
            }

            return Json(new { labels = labels, data = data });
        }
    }
}