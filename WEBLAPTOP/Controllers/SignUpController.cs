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
            kh_new.TK = Request["TK"];
            kh_new.MK = Request["MK"];
            kh_new.TenKH = Request["TenKH"];
            kh_new.DiaChi = Request["DiaChi"];
            kh_new.SDT = Request["SDT"];
            kh_new.Email = Request["Email"];
            kh_new.GioTinh = Request["GioTinh"];
            kh_new.PhanQuyen = 1;

            DateTime date;
            if (DateTime.TryParse(Request["NgaySinh"], out date))
                kh_new.NgaySinh = date;

            db.KHACHHANGs.Add(kh_new);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Login");

        }
    }
}