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

            AddReminderTask();
        }

        public void Session_Start()
        {
            // Initialization
            HttpContext.Current.Session["WebSiteOnline"] = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["WebSiteOnline"]);
            HttpContext.Current.Session["URLWowAPI"] = System.Configuration.ConfigurationManager.AppSettings["URLWowAPI"].ToString();
            HttpContext.Current.Session["APIKey"] = System.Configuration.ConfigurationManager.AppSettings["APIKey"].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddReminderTask()
        {
            OnCacheRemove = new System.Web.Caching.CacheItemRemovedCallback(CacheItemRemoved);

            // Figure out the time for next Scan...
            //   Each day there will be 4 auto scans:
            //   1.  7am
            //   2. 12pm
            //   3.  7pm
            //   4. 12am
            int seconds = 0;
            DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Central Standard Time");

            if (centralDateTime.Hour < 7)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 7, 0, 0) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }
            else if (centralDateTime.Hour >= 7 && centralDateTime.Hour < 12)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 12, 0, 0).AddDays(1) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }
            else if (centralDateTime.Hour >= 12 && centralDateTime.Hour < 19)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 19, 0, 0).AddDays(1) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }
            else if (centralDateTime.Hour >= 19)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 24, 0, 0).AddDays(1) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds);
            }
            else
            {
                // Do it immediately
                seconds = 1;
            }

            // Store it
            System.Configuration.ConfigurationManager.AppSettings["NextGuildScan"] = centralDateTime.AddSeconds(seconds).ToString();

            // Now set the cache for the future
            HttpRuntime.Cache.Insert("RescanGuild", seconds, null, centralDateTime.AddSeconds(seconds), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        /// <param name="r"></param>
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
