using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WGO
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "CharacterName",
                url: "WGO/Character/{name}/{realm}",
                defaults: new { controller = "WGO", action = "Character", name = UrlParameter.Optional, realm = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Rescan",
                url: "WGO/Rescan/{name}/{realm}",
                defaults: new { controller = "WGO", action = "Rescan", name = UrlParameter.Optional, realm = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Delete",
                url: "WGO/Delete/{name}/{realm}",
                defaults: new { controller = "WGO", action = "Delete", name = UrlParameter.Optional, realm = UrlParameter.Optional }
                );

            // Default Route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}
