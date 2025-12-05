using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using WEBLAPTOP.Models;
using System.Data.Entity; // Cần thiết cho FirstOrDefaultAsync

namespace WEBLAPTOP.Controllers
{
    public class SignUpController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        // GET: SignUp (Trang Đăng ký)
        public ActionResult Index()
        {
            // Reset ViewBag.Error khi tải trang lần đầu (không phải lỗi từ Post)
            if (ViewBag.Error == null)
            {
                ViewBag.Error = "";
            }

            // Nếu có lỗi từ quá trình ConfirmOtp hết hạn, nó vẫn nằm trong TempData.
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            return View();
        }

        // B1: Nhận thông tin đăng ký → Tạo OTP → Lưu Cookie → Gửi Email
        [HttpPost]
        public async Task<ActionResult> CreateAccount()
        {
            // Lấy dữ liệu đăng ký từ Request
            string username = Request["TK"];
            string email = Request["Email"];

            // 1. Kiểm tra Tên đăng nhập (TK) đã tồn tại chưa
            var existingUserByTK = await db.KHACHHANGs.FirstOrDefaultAsync(kh => kh.TK == username);
            if (existingUserByTK != null)
            {
                // Thay đổi: Dùng ViewBag.Error và return View("Index") để hiển thị lỗi ngay
                ViewBag.Trangthai = $"Tên tài khoản '{username}' đã có người sử dụng. Vui lòng chọn tên khác.";
                return View("Index");
            }

            // 2. Kiểm tra Email đã tồn tại chưa
            var existingUserByEmail = await db.KHACHHANGs.FirstOrDefaultAsync(kh => kh.Email == email);
            if (existingUserByEmail != null)
            {
                // Thay đổi: Dùng ViewBag.Error và return View("Index") để hiển thị lỗi ngay
                ViewBag.Trangthai = $"Email '{email}' đã được đăng ký. Vui lòng sử dụng Email khác.";
                return View("Index");
            }

            // Lấy toàn bộ dữ liệu đăng ký (Sau khi đã kiểm tra trùng lặp thành công)
            string data =
                $"{username}|{Request["MK"]}|{Request["TenKH"]}|{Request["DiaChi"]}|" +
                $"{Request["SDT"]}|{Request["Email"]}|{Request["GioTinh"]}|{Request["NgaySinh"]}";

            // Mã hóa dữ liệu đăng ký rồi lưu vào cookie
            string encryptedData = CryptoHelper.Encrypt(data);
            HttpCookie infoCookie = new HttpCookie("SIGNUP_INFO", encryptedData);
            infoCookie.HttpOnly = true;
            infoCookie.Expires = DateTime.Now.AddMinutes(10);
            Response.Cookies.Add(infoCookie);

            // Tạo OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // Lưu OTP cookie
            string otpData = $"{otp}|{DateTime.Now.AddMinutes(5)}";
            string encryptedOtp = CryptoHelper.Encrypt(otpData);

            HttpCookie otpCookie = new HttpCookie("OTP_CODE", encryptedOtp);
            otpCookie.HttpOnly = true;
            otpCookie.Expires = DateTime.Now.AddMinutes(5);
            Response.Cookies.Add(otpCookie);

            // Gửi OTP qua email
            await EmailService.Send(email, "Mã OTP xác thực",
                 $"Mã OTP của bạn là: {otp}");

            // Gửi thông báo thành công đến ConfirmOtp.cshtml
            TempData["SuccessMessage"] = $"Mã xác thực (OTP) đã được gửi đến email **{email}**. Vui lòng kiểm tra hộp thư đến (hoặc thư mục Spam) để nhận mã.";

            // Chuyển đến trang nhập OTP
            return RedirectToAction("ConfirmOtp");
        }

        // B2: Trang nhập OTP
        public ActionResult ConfirmOtp()
        {
            // Lấy lỗi từ TempData (thường là lỗi hết hạn cookie từ bước B3)
            if (TempData["Error"] != null)
            {
                ViewBag.Error = TempData["Error"];
            }

            // Lấy thông báo thành công từ TempData (từ bước B1)
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.Success = TempData["SuccessMessage"];
            }
            return View();
        }

        // B3: Xác nhận OTP → tạo tài khoản sau khi OTP đúng
        [HttpPost]
        public async Task<ActionResult> ConfirmOtp(string otp)
        {
            HttpCookie otpCookie = Request.Cookies["OTP_CODE"];
            HttpCookie infoCookie = Request.Cookies["SIGNUP_INFO"];

            if (otpCookie == null || infoCookie == null)
            {
                // Phiên hết hạn hoặc cookie bị mất. Dùng TempData vì cần chuyển hướng.
                TempData["Error"] = "Phiên đăng ký đã hết hạn hoặc cookie bị mất.";
                return RedirectToAction("Index");
            }

            // Giải mã OTP
            string decrypted = CryptoHelper.Decrypt(otpCookie.Value);
            string[] arr = decrypted.Split('|');

            string otpCode = arr[0];
            DateTime expire = DateTime.Parse(arr[1]);

            if (DateTime.Now > expire)
            {
                ViewBag.Error = "OTP đã hết hạn. Vui lòng đăng ký lại.";
                // Xóa cả hai cookie sau khi hết hạn
                otpCookie.Expires = DateTime.Now.AddDays(-1);
                infoCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(otpCookie);
                Response.Cookies.Add(infoCookie);
                return View();
            }

            if (otp != otpCode)
            {
                ViewBag.Error = "OTP không chính xác.";
                return View();
            }

            // === OTP đúng → Tạo tài khoản ===
            string decryptedInfo = CryptoHelper.Decrypt(infoCookie.Value);
            string[] info = decryptedInfo.Split('|');

            KHACHHANG kh_new = new KHACHHANG
            {
                TK = info[0],
                MK = info[1],
                TenKH = info[2],
                DiaChi = info[3],
                SDT = info[4],
                Email = info[5],
                GioTinh = info[6],
                PhanQuyen = 2
            };

            // Ngày sinh
            DateTime date;
            if (DateTime.TryParse(info[7], out date))
                kh_new.NgaySinh = date;

            // Lưu DB
            db.KHACHHANGs.Add(kh_new);
            await db.SaveChangesAsync();

            // Xóa cookies
            otpCookie.Expires = DateTime.Now.AddDays(-1);
            infoCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(otpCookie);
            Response.Cookies.Add(infoCookie);

            // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
            return RedirectToAction("Index", "Login");
        }
    }
}