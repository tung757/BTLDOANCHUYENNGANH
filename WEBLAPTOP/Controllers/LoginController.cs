using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Controllers
{
    public class LoginController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Trangthai = "";
            return View();
        }
        public async Task<ActionResult> Account()
        {
            string username = Request["username"];
            string password = Request["password"];
            string trangthai = "";
            var query = await db.KHACHHANGs.SingleOrDefaultAsync(kh=> kh.TK==username&&kh.MK==password);
            if (query == null) {
                ViewBag.Trangthai = "Thong tin tai khoan khong chinh xac";
                return View("Index");
            }
            else
            {
                Session["username"] = username;
                Session["Rolw"] = query.PhanQuyen;
                return Redirect("~/Home/Index");
            }
            
        }

    }
}