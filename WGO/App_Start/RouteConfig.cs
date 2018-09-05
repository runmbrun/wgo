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
                //url: "WGO/Character/{name}",
                url: "WGO/Character/{name}",
                defaults: new { controller = "WGO", action = "Character", name = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "RaidReload",
                //url: "WGO/Raid/{action}/{name}/{realm}",
                url: "WGO/RaidFunctions/{function}/{name}/{realm}",
                defaults: new { controller = "WGO", action = "RaidFunctions", function = UrlParameter.Optional, name = UrlParameter.Optional, realm = UrlParameter.Optional }
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
