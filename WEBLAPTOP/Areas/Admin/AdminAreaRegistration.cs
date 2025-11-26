using System.Web.Mvc;

namespace WEBLAPTOP.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "SANPHAMs", action = "Index", id = UrlParameter.Optional },
                new[] { "WEBLAPTOP.Areas.Admin.Controllers" }
            );
        }
    }
}