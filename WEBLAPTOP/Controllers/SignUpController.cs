using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Controllers
{
    public class SignUpController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }

        // B1: Nhận thông tin đăng ký → Tạo OTP → Lưu Cookie → Gửi Email
        [HttpPost]
        public async Task<ActionResult> CreateAccount()
        {
            // Lấy dữ liệu đăng ký
            string data =
                $"{Request["TK"]}|{Request["MK"]}|{Request["TenKH"]}|{Request["DiaChi"]}|" +
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
            await EmailService.Send(Request["Email"], "Mã OTP xác thực",
                 $"Mã OTP của bạn là: {otp}");

            // Chuyển đến trang nhập OTP
            return RedirectToAction("ConfirmOtp");
        }

        // B2: Trang nhập OTP
        public ActionResult ConfirmOtp()
        {
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
                ViewBag.Error = "OTP đã hết hạn hoặc cookie bị mất.";
                return View();
            }

            // Giải mã OTP
            string decrypted = CryptoHelper.Decrypt(otpCookie.Value);
            string[] arr = decrypted.Split('|');

            string otpCode = arr[0];
            DateTime expire = DateTime.Parse(arr[1]);

            if (DateTime.Now > expire)
            {
                ViewBag.Error = "OTP đã hết hạn.";
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

            return RedirectToAction("Index", "Login");
        }
    }
}
