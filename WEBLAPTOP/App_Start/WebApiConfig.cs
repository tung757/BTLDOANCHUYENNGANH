using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace WEBLAPTOP.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Cho phép route dạng attribute
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            // Bật CORS (mọi domain, mọi header, mọi method)
            var cors = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Route mặc định: /api/controller/id
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Trả JSON thay vì XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}