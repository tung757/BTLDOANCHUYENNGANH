using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Cần thiết cho .ToListAsync(), .FirstOrDefaultAsync()
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Cần thiết cho Task
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;
using WEBLAPTOP.Models;
using OfficeOpenXml;
using Rotativa;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class DONHANGsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DONHANGs
        public async Task<ActionResult> Index()
        {
            var dONHANGs = db.DONHANGs.Include(d => d.KHACHHANG).Include(d => d.KHUYENMAI);
            return View(await dONHANGs.ToListAsync());
        }

        // GET: Admin/DONHANGs/Details/5
        // GET: Admin/DONHANGs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DONHANG dONHANG = await db.DONHANGs
                .Include(d => d.KHACHHANG)
                .Include(d => d.KHUYENMAI)
                .Include(d => d.DONHANG_SANPHAM.Select(dsp => dsp.SANPHAM))
                .FirstOrDefaultAsync(d => d.ID_DH == id);

            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            return View(dONHANG);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateTrangThai(int id, string trangThai)
        {
            try
            {
                var allowedStatus = new List<string> { "Đang xác nhận", "Đang giao", "Đã giao", "Đã huỷ" };
                if (string.IsNullOrEmpty(trangThai) || !allowedStatus.Contains(trangThai))
                {
                    return Json(new { success = false, message = "Trạng thái không hợp lệ." });
                }

                var dONHANG = await db.DONHANGs.FindAsync(id);

                if (dONHANG == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
                }

                dONHANG.TrangThai = trangThai;
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi hệ thống: " + ex.Message });
            }
        }

        // GET: Admin/DONHANGs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DONHANG dONHANG = await db.DONHANGs.FindAsync(id);
            if (dONHANG == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // POST: Admin/DONHANGs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_DH,NgayLap,GhiChu,TrangThai,ID_KH,ID_KM,Ten,DiaChiGiaoHang,SDT,PhuongthucTT,PhuongThucNhanHang")] DONHANG dONHANG) // 
        {
            if (ModelState.IsValid)
            {
                db.Entry(dONHANG).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ID_KH = new SelectList(db.KHACHHANGs, "ID_KH", "TenKH", dONHANG.ID_KH);
            ViewBag.ID_KM = new SelectList(db.KHUYENMAIs, "ID_KM", "Mota", dONHANG.ID_KM);
            return View(dONHANG);
        }

        // GET: Admin/DONHANGs/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var dONHANG = await db.DONHANGs.FindAsync(id);
            if (dONHANG == null)
                return HttpNotFound();

            db.DONHANGs.Remove(dONHANG);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        public void ExportToExcel()
        {
            // Include các bảng liên quan để lấy tên và tính tiền
            var list = db.DONHANGs
                .Include("KHACHHANG")
                .Include("KHUYENMAI")
                .Include("DONHANG_SANPHAM")
                .ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachDonHang");

                // 1. Tạo tiêu đề
                worksheet.Cells[1, 1].Value = "ID Đơn";
                worksheet.Cells[1, 2].Value = "Khách Hàng (TK)";
                worksheet.Cells[1, 3].Value = "Người Nhận";
                worksheet.Cells[1, 4].Value = "SĐT Nhận";
                worksheet.Cells[1, 5].Value = "Địa Chỉ Giao";
                worksheet.Cells[1, 6].Value = "Ngày Lập";
                worksheet.Cells[1, 7].Value = "Trạng Thái";
                worksheet.Cells[1, 8].Value = "PT Thanh Toán";
                worksheet.Cells[1, 9].Value = "PT Nhận Hàng";
                worksheet.Cells[1, 10].Value = "Khuyến Mãi";
                worksheet.Cells[1, 11].Value = "Tổng Tiền"; // Cột quan trọng
                worksheet.Cells[1, 12].Value = "Ghi Chú";

                // Style Header
                using (var range = worksheet.Cells[1, 1, 1, 12])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // 2. Đổ dữ liệu
                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.ID_DH;
                    worksheet.Cells[row, 2].Value = item.KHACHHANG != null ? item.KHACHHANG.TenKH : "Khách vãng lai";
                    worksheet.Cells[row, 3].Value = item.Ten;
                    worksheet.Cells[row, 4].Value = item.SDT;
                    worksheet.Cells[row, 5].Value = item.DiaChiGiaoHang;

                    // Format ngày giờ
                    worksheet.Cells[row, 6].Value = item.NgayLap.HasValue ? item.NgayLap.Value.ToString("dd/MM/yyyy HH:mm") : "";

                    worksheet.Cells[row, 7].Value = item.TrangThai;
                    worksheet.Cells[row, 8].Value = item.PhuongthucTT;
                    worksheet.Cells[row, 9].Value = item.PhuongThucNhanHang;

                    // Tên khuyến mãi
                    worksheet.Cells[row, 10].Value = item.KHUYENMAI != null ? item.KHUYENMAI.Mota : "Không";

                    // TÍNH TỔNG TIỀN (Logic giống hệt trong trang Index/Details)
                    long tongTien = 0;
                    if (item.DONHANG_SANPHAM != null)
                    {
                        tongTien = item.DONHANG_SANPHAM.Sum(x => (long)(x.SoLuong ?? 0) * (x.DonGia ?? 0));
                    }
                    worksheet.Cells[row, 11].Value = tongTien;
                    worksheet.Cells[row, 11].Style.Numberformat.Format = "#,##0"; // Format số tiền trong Excel

                    worksheet.Cells[row, 12].Value = item.GhiChu;

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                // 3. Xuất file
                var stream = new System.IO.MemoryStream();
                package.SaveAs(stream);

                string fileName = "DonHang_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }
        // GET: Admin/DONHANGs/ExportToPDF
        public ActionResult ExportToPDF()
        {
            var list = db.DONHANGs.Include("KHACHHANG").Include("DONHANG_SANPHAM").OrderByDescending(d => d.NgayLap).ToList();

            var pdfResult = new ViewAsPdf("ExportPDF", list)
            {
                FileName = "DanhSachDonHang.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape, // Khổ ngang
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };

            var cookies = Request.Cookies;
            if (cookies != null)
            {
                pdfResult.Cookies = new Dictionary<string, string>();
                foreach (var key in cookies.AllKeys)
                {
                    pdfResult.Cookies.Add(key, cookies[key].Value);
                }
            }

            return pdfResult;
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