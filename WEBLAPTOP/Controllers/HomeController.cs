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
            var lst_sp = await db.SANPHAMs.Where(sp => sp.Status_SP == 1).ToListAsync();
            return View(lst_sp);
        }
    }
}