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
        private static System.Web.Caching.CacheItemRemovedCallback OnCacheRemove = null;
        
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<Models.WGODBContext>(null);

            //todo: AddReminderTask();
        }

        public void Session_Start()
        {
            // Initialization
            HttpContext.Current.Session["WebSiteOnline"] = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["WebSiteOnline"]);
            HttpContext.Current.Session["URLWowAPI"] = System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString();
            HttpContext.Current.Session["APIKey"] = System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString();
        }

        private void AddReminderTask()
        {
            OnCacheRemove = new System.Web.Caching.CacheItemRemovedCallback(CacheItemRemoved);

            // Figure out time for next 8am...
            int seconds = 0;
            if (DateTime.Now.Hour < 8)
            {
                TimeSpan remind = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0) - DateTime.Now;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }
            else
            {
                TimeSpan remind = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0).AddDays(1) - DateTime.Now;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }

            HttpRuntime.Cache.Insert("RescanGuild", seconds, null, DateTime.Now.AddSeconds(seconds), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, System.Web.Caching.CacheItemRemovedReason r)
        {
            // Do a full pull of the guild
            using (Controllers.WGOController wgo = new Controllers.WGOController())
            {
                wgo.RetrieveGuild("Secondnorth", "Thrall");
            }

            // re-add our task so it reoccurs
            AddReminderTask();
        }
    }
}
