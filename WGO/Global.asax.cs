using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WGO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<Models.WGODBContext>(null);
        }

        public void Session_Start()
        {
            // Initialization
            HttpContext.Current.Session["WebSiteOnline"] = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["WebSiteOnline"]);
            HttpContext.Current.Session["URLWowAPI"] = System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString();
            HttpContext.Current.Session["APIKey"] = System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString();
        }
    }
}
