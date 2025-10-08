using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Controllers
{
    public class ProductController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        // GET: Product
        public async Task<ActionResult> Index(ProductFilter filter, int page = 1, int pageSize=6)
        {
            IQueryable<SANPHAM> ds = db.SANPHAMs.AsQueryable();

            switch (filter.sort)
            {
                case "price_asc":
                    ds = ds.OrderBy(p => p.Gia);
                    break;
                case "price_desc":
                    ds = ds.OrderByDescending(p => p.Gia);
                    break;
                default:
                    ds = ds.OrderByDescending(p => p.SoLuongBan);
                    break;
            }

            if(filter.display== "sell_percent")
            {
                ds = ds.Where(p => p.Gia > p.GiaBan);
            }


            ds = ds.Skip((page - 1) * pageSize).Take(pageSize);

            var products = await ds.ToListAsync();

            int totalItems = await db.SANPHAMs.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentSort = filter.sort;
            ViewBag.CurrentDisplay = filter.display;

            return View(products);
        }
        public async Task<ActionResult> Details(int id)
        {
            var query = await db.SANPHAMs.SingleOrDefaultAsync(sp=> sp.ID_SP == id);
            return View(query);
        }
    }
}