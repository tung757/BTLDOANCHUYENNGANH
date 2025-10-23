using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    public class DANHMUCsController : Controller
    {
        private readonly DARKTHESTORE db = new DARKTHESTORE();

        // GET: Admin/DANHMUCs
        public async Task<ActionResult> Index()
        {
            var danhMucs = await db.DANHMUCs.ToListAsync();
            return View(danhMucs);
        }

        // GET: Admin/DANHMUCs/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.DanhMucList = await db.DANHMUCs.ToListAsync();
            return View();
        }

        // POST: Admin/DANHMUCs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.DANHMUCs.Add(dANHMUC);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucList = await db.DANHMUCs.ToListAsync();
            return View(dANHMUC);
        }

        // GET: Admin/DANHMUCs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dANHMUC = await db.DANHMUCs.FindAsync(id);
            if (dANHMUC == null)
                return HttpNotFound();

            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID_DM,TenDM")] DANHMUC dANHMUC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dANHMUC).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dANHMUC);
        }

        // POST: Admin/DANHMUCs/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var dANHMUC = await db.DANHMUCs.FindAsync(id);
            if (dANHMUC == null)
                return HttpNotFound();

            db.DANHMUCs.Remove(dANHMUC);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
