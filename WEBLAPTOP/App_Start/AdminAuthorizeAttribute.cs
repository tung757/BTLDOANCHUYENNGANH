using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.App_Start
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Lấy Role từ Session (Dựa theo code Login của bạn: Session["Role"])
            var roleSession = HttpContext.Current.Session["Role"];

            // 2. Kiểm tra đăng nhập: Nếu Session Null (Chưa đăng nhập)
            if (roleSession == null)
            {
                // Đá về trang Đăng nhập (Account/Login hoặc Account/Index)
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Index",
                        area = ""
                    }));
                return;
            }

            // 3. Kiểm tra quyền:
            int userRole = int.Parse(roleSession.ToString());

            if (userRole != 1)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index",
                        area = ""
                    }));
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}