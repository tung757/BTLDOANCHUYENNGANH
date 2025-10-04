using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
        [HttpPost]
        public async Task<ActionResult> CreateAccount()
        {
            KHACHHANG kh_new = new KHACHHANG();
            kh_new.TK = Request["username"].ToString();
            kh_new.MK = Request["password"].ToString();
            kh_new.TenKH= Request["fullname"].ToString();
            kh_new.DiaChi = Request["address"].ToString();
            kh_new.SDT = Request["phone_number"].ToString();
            kh_new.NgaySinh = Convert.ToDateTime(Request["both"]);
            kh_new.Email= Request["email"].ToString();
            kh_new.GioTinh = Request["sex"].ToString();
            kh_new.PhanQuyen = 1;
            db.KHACHHANGs.Add(kh_new);
            db.SaveChanges();
            return RedirectToAction("Index","Login");
        }
    }
}