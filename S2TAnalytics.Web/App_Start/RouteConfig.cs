using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace S2TAnalytics.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Admin",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapHttpRoute(
            //name: "EmailConfirmed",
            //routeTemplate: "api/{controller}/{action}/{email}/{code}",
            //defaults: new { id = RouteParameter.Optional });
            //routes.MapRoute(
            //    name: "AccountIndex",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    name: "signup",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Account", action = "SignUp", id = UrlParameter.Optional }
            //);
        }
    }
}
