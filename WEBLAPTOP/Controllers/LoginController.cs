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
        public ActionResult Index()
        {
            ViewBag.Trangthai = "";
            return View();
        }
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
                Session["username"] = username;
                Session["id"] = query.ID_KH;
                Session["Role"] = query.PhanQuyen;

                if (query.PhanQuyen == 3)
                {
                    return RedirectToAction("Index", "HomeA", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

            }

        }
        public ActionResult Logout()
        {
            Session.Clear();
            return View("Index");
        }

    }
}