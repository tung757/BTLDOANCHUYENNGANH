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
        public async Task<ActionResult> Index(ProductFilter filter, int page = 1, int pageSize = 6)
        {
            //lấy danh sách danh mục
            List<DANHMUC> lstcategori = await db.DANHMUCs.ToListAsync();
            ViewBag.categories = lstcategori;

            IQueryable<SANPHAM> ds = db.SANPHAMs.AsQueryable();

            //lọc theo danh mục
            if (filter.categories_id != null)
            {
                ds = ds.Where(sp => sp.ID_DM == filter.categories_id);
            }

            //lọc theo giá
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

            //lọc theo giảm giá
            if (filter.display == "sell_percent")
            {
                ds = ds.Where(p => p.Gia > p.GiaBan);
            }

            if (filter.price_start != null)
            {
                ds = ds.Where(p => p.GiaBan >= filter.price_start);
            }
            if (filter.price_end != null)
            {
                ds = ds.Where(p => p.GiaBan <= filter.price_end);
            }

            var dssl = ds;

            //phân trang
            ds = ds.Skip((page - 1) * pageSize).Take(pageSize);

            var products = await ds.ToListAsync();

            int totalItems = await dssl.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentSort = filter.sort;
            ViewBag.CurrentDisplay = filter.display;
            ViewBag.Curentcategori = filter.categories_id;

            return View(products);
        }
        public async Task<ActionResult> Details(int id)
        {
            var query = await db.SANPHAMs.SingleOrDefaultAsync(sp => sp.ID_SP == id);
            return View(query);
        }
    }
}