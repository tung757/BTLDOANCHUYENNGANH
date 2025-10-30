    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;
using WEBLAPTOP.ViewModel;

namespace WEBLAPTOP.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private readonly DARKTHESTORE db = new DARKTHESTORE();
        public ActionResult Index()
        {
            return View();
        }
    }
}