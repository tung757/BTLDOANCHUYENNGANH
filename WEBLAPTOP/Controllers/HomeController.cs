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
    public class HomeController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        public async Task<ActionResult> Index()
        {
            var banners = db.QUANGCAOs
                    .Where(x => x.Loai == "Banner")
                    .OrderByDescending(x => x.Id)
                    .ToList();

            ViewBag.SliderBanners = banners;
            var lst_sp = await db.SANPHAMs
                                 .Where(sp => sp.Status_SP == 1)
                                 .OrderByDescending(sp => sp.NgayTao) // Nên sắp xếp sp mới nhất lên đầu
                                 .ToListAsync();

            return View(lst_sp);
        }
    }
}