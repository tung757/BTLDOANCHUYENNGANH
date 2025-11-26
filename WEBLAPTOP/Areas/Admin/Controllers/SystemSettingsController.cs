using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.App_Start;

namespace WEBLAPTOP.Areas.Admin.Controllers
{
    [AdminAuthorize]

    public class SystemSettingsController : Controller
    {
        // GET: Admin/SystemSettings
        public ActionResult Index()
        {
            return View();
        }
    }
}