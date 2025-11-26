using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeAController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}