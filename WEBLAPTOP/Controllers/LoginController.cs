using Newtonsoft.Json; // Cần thiết để đọc dữ liệu JSON từ Google
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net; // <== DÒNG THÊM MỚI QUAN TRỌNG
using System.Net.Http; // Cần thiết cho việc gọi Google
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using System.Data.Entity.Validation;
namespace WEBLAPTOP.Controllers
{
    public class LoginController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        // --- CẤU HÌNH GOOGLE ---
        private string ClientId => System.Configuration.ConfigurationManager.AppSettings["GoogleClientId"];
        private string ClientSecret => System.Configuration.ConfigurationManager.AppSettings["GoogleClientSecret"];
        private string RedirectUri => System.Configuration.ConfigurationManager.AppSettings["GoogleRedirectUri"];


        public ActionResult Index()
        {
            ViewBag.Trangthai = "";
            return View();
        }

        // --- LOGIC ĐĂNG NHẬP THƯỜNG ---
        [HttpPost]
        public async Task<ActionResult> Account()
        {
            string username = Request["username"];
            string password = Request["password"];
            var query = await db.KHACHHANGs.SingleOrDefaultAsync(kh => kh.TK == username && kh.MK == password);

            if (query == null)
            {
                ViewBag.Trangthai = "Thông tin tài khoản không chính xác";
                return View("Index");
            }
            else
            {
                SetSession(query);
                return RedirectToRole(query.PhanQuyen);
            }
        }

        // --- LOGIC ĐĂNG NHẬP GOOGLE ---
        // 1. Gửi người dùng sang trang đăng nhập Google
        public ActionResult LoginWithGoogle()
        {
            var googleUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={ClientId}&response_type=code&redirect_uri={RedirectUri}&scope=email%20profile&prompt=select_account";
            //                                                                                 ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ <-- DÒNG ĐÃ THÊM
            return Redirect(googleUrl);
        }

        public async Task<ActionResult> GoogleCallback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Trangthai = "Đăng nhập Google thất bại (Không có code).";
                return View("Index");
            }

            try
            {
                // === FIX LỖI KẾT NỐI (TLS/SSL) ===
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                // ===================================

                // Bước A: Đổi Code lấy Access Token
                var tokenUrl = "https://oauth2.googleapis.com/token";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                });

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(tokenUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        // Nếu lỗi là 400 (Bad Request), vẫn là do Client ID/Secret/Redirect URI sai
                        ViewBag.Trangthai = $"Lỗi xác thực Google: {response.StatusCode}. Vui lòng kiểm tra lại cấu hình Client ID/Secret trên Google Cloud.";
                        return View("Index");
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var tokenData = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseString);

                    // Bước B: Dùng Token để lấy thông tin User (Email, Tên...)
                    var userInfoUrl = $"https://www.googleapis.com/oauth2/v2/userinfo?access_token={tokenData.access_token}";
                    var userResponse = await client.GetAsync(userInfoUrl);
                    if (!userResponse.IsSuccessStatusCode) return RedirectToAction("Index");

                    var userString = await userResponse.Content.ReadAsStringAsync();
                    var googleUser = JsonConvert.DeserializeObject<GoogleUserProfile>(userString);

                    // Bước C: Xử lý đăng nhập vào hệ thống DB của bạn
                    return await ProcessGoogleLogin(googleUser);
                }
            }
            catch (Exception ex)
            {
                // Đây là lỗi kết nối
                ViewBag.Trangthai = $"Lỗi kết nối đến Google. Chi tiết: {ex.Message}";
                return View("Index");
            }
        }

        private async Task<ActionResult> ProcessGoogleLogin(GoogleUserProfile googleUser)
        {
            // 1. Tìm khách hàng theo Email (TK)
            var khachhang = await db.KHACHHANGs.FirstOrDefaultAsync(kh => kh.Email == googleUser.email);

            if (khachhang != null)
            {
                // --- TRƯỜNG HỢP 1: Đã có tài khoản --> Đăng nhập luôn ---
                SetSession(khachhang);
                return RedirectToRole(khachhang.PhanQuyen);
            }
            else
            {
                // --- TRƯỜNG HỢP 2: Chưa có tài khoản -> TỰ ĐỘNG TẠO MỚI ---

                khachhang = new KHACHHANG();
                khachhang.TK = googleUser.email.Substring(0, 20);
                khachhang.Email = googleUser.email;
                khachhang.TenKH = googleUser.name;
                khachhang.MK = Guid.NewGuid().ToString().Substring(0, 20);
                khachhang.PhanQuyen = 2;



                db.KHACHHANGs.Add(khachhang);

                try
                {
                    await db.SaveChangesAsync(); // Dòng này được bảo vệ bởi try-catch
                }
                catch (DbEntityValidationException ex)
                {
                    // === HIỂN THỊ CHI TIẾT LỖI THIẾU CỘT ĐỂ DEBUG ===
                    string errorDetails = "Lỗi Database: Các cột NOT NULL đang bị thiếu giá trị: ";
                    foreach (var entity in ex.EntityValidationErrors)
                    {
                        foreach (var error in entity.ValidationErrors)
                        {
                            errorDetails += $"[{error.PropertyName} - {error.ErrorMessage}]; ";
                        }
                    }
                    ViewBag.Trangthai = errorDetails;
                    return View("Index");
                    // ===============================================
                }

                // Đăng nhập sau khi tạo thành công (Chỉ chạy khi SaveChanges thành công)
                SetSession(khachhang);
                return RedirectToRole(khachhang.PhanQuyen);
            }
        }

        // --- CÁC HÀM HỖ TRỢ ĐỂ TRÁNH LẶP CODE ---

        private void SetSession(KHACHHANG kh)
        {
            Session["username"] = kh.TK;
            Session["id"] = kh.ID_KH;
            Session["Role"] = kh.PhanQuyen;
        }

        private ActionResult RedirectToRole(int? phanQuyen)
        {
            if (phanQuyen == 1)
            {
                return RedirectToAction("Index", "HomeA", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

    // --- CÁC CLASS HỨNG DỮ LIỆU JSON TỪ GOOGLE ---
    public class GoogleTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string id_token { get; set; }
    }

    public class GoogleUserProfile
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public bool verified_email { get; set; }
    }
}