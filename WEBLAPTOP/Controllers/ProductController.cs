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
    public class ProductController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        // GET: Product
        public async Task<ActionResult> Index()
        {
            IEnumerable<SANPHAM> ds = await db.SANPHAMs.ToListAsync();
            return View(ds);
        }
        public async Task<ActionResult> Details(int id)
        {
            var query = await db.SANPHAMs.SingleOrDefaultAsync(sp=> sp.ID_SP == id);
            return View(query);
        }
    }
}