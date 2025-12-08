using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity.Infrastructure;
using WEBLAPTOP.App_Start;
using OfficeOpenXml;
using Rotativa;
using System.Web.UI;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class KHACHHANGsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/KHACHHANGs (Async)
        public async Task<ActionResult> Index(int page=1, int page_size=10, string search="")
        {
            // Dùng ToListAsync()
            var khs = await db.KHACHHANGs.ToListAsync();
            if (search.Trim() != "")
            {
                khs = khs.Where(k => k.TenKH.Contains(search)).ToList();
            }
            int total_items = khs.Count();
            int total_pages = (int)Math.Ceiling((double)total_items / page_size);
            var kh_page = khs.OrderBy(k => k.TenKH).Skip((page-1)*page_size).Take(page_size).ToList();
            ViewBag.Page = page;
            ViewBag.TotalPages = total_pages;
            return View(kh_page);
        }

        // GET: Admin/KHACHHANGs/Details/5 (Async)
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Dùng FindAsync()
            KHACHHANG kHACHHANG = await db.KHACHHANGs.FindAsync(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }
        // GET: Admin/KHACHHANGs/Edit/5 (Async)
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Dùng FindAsync()
            KHACHHANG kHACHHANG = await db.KHACHHANGs.FindAsync(id);
            if (kHACHHANG == null)
            {
                return HttpNotFound();
            }
            return View(kHACHHANG);
        }

        // POST: Admin/KHACHHANGs/Edit/5 (Async)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_KH,TenKH,DiaChi,SDT,GioTinh,NgaySinh,Email,TK,MK,PhanQuyen")] KHACHHANG kHACHHANG)
        {
            if (ModelState.IsValid)
            {
                // Xử lý Mật khẩu: Chỉ cập nhật nếu có nhập mới
                if (string.IsNullOrEmpty(kHACHHANG.MK))
                {
                    db.Entry(kHACHHANG).State = EntityState.Modified;
                    db.Entry(kHACHHANG).Property(x => x.MK).IsModified = false; // Không sửa MK
                }
                else
                {
                    // (!!! LƯU Ý BẢO MẬT: Nên mã hóa MK mới ở đây !!!)
                    db.Entry(kHACHHANG).State = EntityState.Modified; // Sửa bình thường (bao gồm MK)
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(kHACHHANG);
        }

        // POST: Admin/KHACHHANGs/Delete/5 (Đã Async từ trước)
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var kHACHHANG = await db.KHACHHANGs.FindAsync(id);
                if (kHACHHANG == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khách hàng." });
                }

                db.KHACHHANGs.Remove(kHACHHANG);
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (DbUpdateException) // Lỗi khóa ngoại
            {
                return Json(new { success = false, message = "Không thể xóa! Khách hàng này đã có dữ liệu liên quan (đơn hàng, giỏ hàng, đánh giá)." });
            }
            catch (Exception ex) // Lỗi chung
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        public void ExportToExcel()
        {
            var list = db.KHACHHANGs.ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachKhachHang");

                // 1. Tạo tiêu đề
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Tên Khách Hàng";
                worksheet.Cells[1, 3].Value = "Tài Khoản";
                // Bỏ Mật Khẩu vì lý do bảo mật
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "SĐT";
                worksheet.Cells[1, 6].Value = "Giới Tính";
                worksheet.Cells[1, 7].Value = "Ngày Sinh";
                worksheet.Cells[1, 8].Value = "Địa Chỉ";
                worksheet.Cells[1, 9].Value = "Phân Quyền";

                // Style cho Header
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // 2. Đổ dữ liệu
                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.ID_KH;
                    worksheet.Cells[row, 2].Value = item.TenKH;
                    worksheet.Cells[row, 3].Value = item.TK;
                    worksheet.Cells[row, 4].Value = item.Email;
                    // Ép kiểu chuỗi cho SĐT để tránh mất số 0 ở đầu
                    worksheet.Cells[row, 5].Value = item.SDT;
                    worksheet.Cells[row, 6].Value = item.GioTinh;

                    // Format ngày sinh
                    worksheet.Cells[row, 7].Value = item.NgaySinh.HasValue ? item.NgaySinh.Value.ToString("dd/MM/yyyy") : "";

                    worksheet.Cells[row, 8].Value = item.DiaChi;

                    // Format Phân quyền cho dễ hiểu
                    worksheet.Cells[row, 9].Value = (item.PhanQuyen == 1) ? "Admin" : "Khách hàng";

                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                // 3. Xuất file
                var stream = new System.IO.MemoryStream();
                package.SaveAs(stream);

                string fileName = "KhachHang_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }
        // GET: Admin/KHACHHANGs/ExportToPDF
        public ActionResult ExportToPDF()
        {
            var list = db.KHACHHANGs.OrderBy(k => k.TenKH).ToList();

            var pdfResult = new ViewAsPdf("ExportPDF", list)
            {
                FileName = "DanhSachKhachHang.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Portrait, // Khổ Dọc
                PageMargins = new Rotativa.Options.Margins(15, 15, 15, 15)
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
        // Dispose (Không đổi)
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