using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity; // Cần cho .ToListAsync(), .FindAsync(), ...
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks; // Cần cho Task<>
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;
using WEBLAPTOP.Models;
using OfficeOpenXml;
using Rotativa;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SANPHAMsController : Controller
    {
        private DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/SANPHAMs
        public async Task<ActionResult> Index(int page = 1, int page_size = 6, string search="")
        {
            var query = db.SANPHAMs.Include(s => s.DANHMUC);

            int total_items = await query.CountAsync();
            if (search.Trim() != "")
            {
                query = query.Where(sp => sp.TenSP.Contains(search));
            }
            int total_pages = (int)Math.Ceiling((double)total_items / page_size);

            //order
            var results_page = await query
                .OrderBy(sp => sp.TenSP)
                .Skip((page - 1) * page_size)
                .Take(page_size)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = total_pages;

            return View(results_page);
        }

        // GET: Admin/SANPHAMs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // ĐÃ SỬA: Dùng FindAsync
            SANPHAM sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,ID_DM")] SANPHAM sANPHAM, IEnumerable<HttpPostedFileBase> ImagesFile)
        {
            // ... (Phần kiểm tra trùng Mã SP giữ nguyên) ...

            if (ModelState.IsValid)
            {
                sANPHAM.NgayTao = DateTime.Now;

                // XỬ LÝ ẢNH
                if (ImagesFile != null && ImagesFile.FirstOrDefault() != null)
                {
                    var listTenAnh = new List<string>();

                    // 1. Lấy tên sản phẩm để làm tên file
                    string productName = sANPHAM.TenSP ?? "san-pham";

                    // 2. "Làm sạch" tên (Tiếng Việt có dấu -> không dấu, khoảng trắng -> gạch ngang)
                    // Bạn có thể dùng hàm chuyển đổi Tiếng Việt không dấu nếu muốn kỹ hơn
                    string invalidChars = new string(Path.GetInvalidFileNameChars());
                    string sanitizedName = new string(productName.Where(ch => !invalidChars.Contains(ch)).ToArray())
                                            .Replace(" ", "-").ToLower(); // Ví dụ: "Laptop Dell" -> "laptop-dell"

                    string folderPath = Server.MapPath("~/Images/Product_images/");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    // 3. Duyệt qua từng file và đặt tên
                    int count = 1; // Biến đếm để đánh số ảnh (1, 2, 3...)
                    foreach (var file in ImagesFile)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(file.FileName);

                            // TẠO TÊN FILE: ten-sp-1.jpg, ten-sp-2.jpg
                            string fileName = $"{sanitizedName}-{count}{extension}";

                            // KIỂM TRA TRÙNG: Nếu file đã tồn tại (do sp khác trùng tên), thêm số ngẫu nhiên để không đè
                            string fullPath = Path.Combine(folderPath, fileName);
                            while (System.IO.File.Exists(fullPath))
                            {
                                // Đổi tên thành: ten-sp-1-random.jpg
                                fileName = $"{sanitizedName}-{count}-{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
                                fullPath = Path.Combine(folderPath, fileName);
                            }

                            // Lưu file
                            file.SaveAs(fullPath);

                            // Thêm vào danh sách
                            listTenAnh.Add(fileName);

                            count++; // Tăng số thứ tự
                        }
                    }

                    // Nối chuỗi bằng dấu chấm phẩy
                    if (listTenAnh.Count > 0)
                    {
                        sANPHAM.Images_url = string.Join(";", listTenAnh);
                    }
                }
                else
                {
                    sANPHAM.Images_url = "default.jpg";
                }

                db.SANPHAMs.Add(sANPHAM);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
            {
                return HttpNotFound();
            }
            // ĐÃ SỬA: Dùng ToListAsync()
            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // POST: Admin/SANPHAMs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_SP,MaSP,TenSP,Gia,GiaBan,Mota,Status_SP,NgayTao,SoLuong,SoLuongBan,ID_DM")] SANPHAM sANPHAM, IEnumerable<HttpPostedFileBase> ImagesFile, string ImagesToDelete)
        {
            if (ModelState.IsValid)
            {
                var sanPhamCu = await db.SANPHAMs.AsNoTracking().FirstOrDefaultAsync(x => x.ID_SP == sANPHAM.ID_SP);
                string currentImagesString = sanPhamCu.Images_url ?? "";

                // Biến danh sách ảnh thành List để dễ thao tác (Xóa/Thêm)
                var listAnhHienTai = new List<string>();
                if (!string.IsNullOrEmpty(currentImagesString) && currentImagesString != "default.jpg")
                {
                    listAnhHienTai = currentImagesString.Split(';').ToList();
                }

                string folderPath = Server.MapPath("~/Images/Product_images/");
                if (!string.IsNullOrEmpty(ImagesToDelete))
                {
                    // View sẽ gửi về dạng: "anh1.jpg,anh2.jpg"
                    var arrToDelete = ImagesToDelete.Split(',');

                    foreach (var imgName in arrToDelete)
                    {
                        if (listAnhHienTai.Contains(imgName))
                        {
                            // A. Xóa tên khỏi danh sách
                            listAnhHienTai.Remove(imgName);

                            // B. Xóa file vật lý (nếu tồn tại và không phải default)
                            string fullPath = Path.Combine(folderPath, imgName);
                            if (System.IO.File.Exists(fullPath) && imgName != "default.jpg")
                            {
                                try { System.IO.File.Delete(fullPath); } catch { }
                            }
                        }
                    }
                }

                // 2. XỬ LÝ THÊM ẢNH MỚI
                if (ImagesFile != null && ImagesFile.FirstOrDefault() != null)
                {
                    // Chuẩn bị tên gốc
                    string productName = sANPHAM.TenSP ?? "san-pham";
                    string sanitizedName = new string(productName.Where(ch => !Path.GetInvalidFileNameChars().Contains(ch)).ToArray()).Replace(" ", "-").ToLower();

                    // Tính số thứ tự tiếp theo (để không trùng với ảnh cũ còn lại)
                    int startCount = listAnhHienTai.Count + 1;

                    foreach (var file in ImagesFile)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            string extension = Path.GetExtension(file.FileName);
                            string fileName = $"{sanitizedName}-{startCount}-{Guid.NewGuid().ToString().Substring(0, 4)}{extension}";
                            string fullPath = Path.Combine(folderPath, fileName);

                            file.SaveAs(fullPath);

                            // Thêm vào danh sách
                            listAnhHienTai.Add(fileName);
                            startCount++;
                        }
                    }
                }

                // 3. CẬP NHẬT DATABASE

                if (listAnhHienTai.Count > 0)
                {
                    sANPHAM.Images_url = string.Join(";", listAnhHienTai);
                }
                else
                {
                    sANPHAM.Images_url = "default.jpg"; // Nếu xóa hết thì về mặc định
                }

                sANPHAM.NgayTao = sanPhamCu.NgayTao;
                db.Entry(sANPHAM).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ID_DM = new SelectList(await db.DANHMUCs.ToListAsync(), "ID_DM", "TenDM", sANPHAM.ID_DM);
            return View(sANPHAM);
        }

        // GET: Admin/SANPHAMs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var sANPHAM = await db.SANPHAMs.FindAsync(id);
            if (sANPHAM == null)
                return HttpNotFound();

            // Ghi chú: Bạn có thể thêm logic xóa file ảnh ở đây
            if (!string.IsNullOrEmpty(sANPHAM.Images_url))
            {
                string absolutePath = Server.MapPath(sANPHAM.Images_url);
                if (System.IO.File.Exists(absolutePath))
                {
                    try { System.IO.File.Delete(absolutePath); }
                    catch (Exception) { /* Bỏ qua */ }
                }
            }

            db.SANPHAMs.Remove(sANPHAM);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        public void ExportToExcel()
        {
            var list = db.SANPHAMs.Include("DANHMUC").ToList();

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DanhSachSanPham");

                // 1. Tạo tiêu đề cột
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Mã SP";
                worksheet.Cells[1, 3].Value = "Tên Sản Phẩm";
                worksheet.Cells[1, 4].Value = "Danh Mục";       // Thay vì hiện ID_DM, ta hiện Tên DM
                worksheet.Cells[1, 5].Value = "Giá Nhập";       // Thuộc tính 'Gia'
                worksheet.Cells[1, 6].Value = "Giá Bán";        // Thuộc tính 'GiaBan'
                worksheet.Cells[1, 7].Value = "Số Lượng Tồn";   // Thuộc tính 'SoLuong'
                worksheet.Cells[1, 8].Value = "Số Lượng Bán";   // Thuộc tính 'SoLuongBan'
                worksheet.Cells[1, 9].Value = "Ngày Tạo";       // Thuộc tính 'NgayTao'
                worksheet.Cells[1, 10].Value = "Mô Tả";

                using (var range = worksheet.Cells[1, 1, 1, 10])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                // 2. Đổ dữ liệu
                int row = 2;
                foreach (var item in list)
                {
                    worksheet.Cells[row, 1].Value = item.ID_SP;
                    worksheet.Cells[row, 2].Value = item.MaSP;
                    worksheet.Cells[row, 3].Value = item.TenSP;

                    // Lấy tên danh mục (Cần đảm bảo bạn đã .Include(s => s.DANHMUC) khi lấy list)
                    worksheet.Cells[row, 4].Value = item.DANHMUC != null ? item.DANHMUC.TenDM : "Không có";

                    // Định dạng số tiền (nếu muốn hiện dấu phẩy trong Excel thì format ở đây hoặc để nguyên số)
                    worksheet.Cells[row, 5].Value = item.Gia;
                    worksheet.Cells[row, 6].Value = item.GiaBan;

                    worksheet.Cells[row, 7].Value = item.SoLuong;
                    worksheet.Cells[row, 8].Value = item.SoLuongBan ?? 0; // Xử lý null nếu có

                    // Định dạng ngày tháng cho đẹp (dd/MM/yyyy)
                    worksheet.Cells[row, 9].Value = item.NgayTao.HasValue ? item.NgayTao.Value.ToString("dd/MM/yyyy") : "";

                    worksheet.Cells[row, 10].Value = item.Mota;

                    row++;
                }

                // 3. Format file (Tự động căn chỉnh độ rộng cột)
                worksheet.Cells.AutoFitColumns();

                // 4. Xuất file
                var stream = new System.IO.MemoryStream();
                package.SaveAs(stream);

                string fileName = "DanhSachSanPham_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                stream.Position = 0;
                // Cách trả về file trong MVC (void action nhưng dùng Response)
                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                Response.BinaryWrite(stream.ToArray());
                Response.End();
            }
        }
        // Action này sẽ in nội dung của trang Index ra PDF
        public ActionResult ExportToPDF()
        {
            var list = db.SANPHAMs.Include("DANHMUC").OrderByDescending(s => s.ID_SP).ToList();
            // Trỏ đến view "ExportPDF"
            var pdfResult = new ViewAsPdf("ExportPDF", list)
            {
                FileName = "DanhSachSanPham.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape, // Khổ ngang
                PageMargins = new Rotativa.Options.Margins(10, 10, 10, 10)
            };

            // 3. Copy Cookie để không bị lỗi đăng nhập
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