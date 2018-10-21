using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WGO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        private static System.Web.Caching.CacheItemRemovedCallback OnCacheRemove = null;
        
        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<Models.WGODBContext>(null);

            // This will setup a caching callback function that will act like a timer.
            //  The timer will be setup to run periodically throughout the day.
            AddReminderTask();
        }

        /// <summary>
        /// 
        /// </summary>
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

            // DEBUGGING
            //centralDateTime = new DateTime(2018, 9, 21, 13, 0, 0);

            if (centralDateTime.Hour >= 0 && centralDateTime.Hour < 7)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 7, 0, 0) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds) + 60;
            }
            else if (centralDateTime.Hour >= 7 && centralDateTime.Hour < 12)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 12, 0, 0) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds) + 60;
            }
            else if (centralDateTime.Hour >= 12 && centralDateTime.Hour < 19)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 19, 0, 0) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds) + 60;
            }
            else if (centralDateTime.Hour >= 19)
            {
                TimeSpan remind = new DateTime(centralDateTime.Year, centralDateTime.Month, centralDateTime.Day, 23, 59, 59) - centralDateTime;
                seconds = Convert.ToInt32(remind.TotalSeconds) + 60;
            }
            else
            {
                // Do it immediately
                seconds = 60;
            }

            // Store it
            System.Configuration.ConfigurationManager.AppSettings["LastGuildScan"] = System.Configuration.ConfigurationManager.AppSettings["NextGuildScan"];
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
            // re-add our task so it reoccurs
            AddReminderTask();

            // Do a full pull of the guild
            using (Controllers.WGOController wgo = new Controllers.WGOController())
            {
                wgo.RetrieveGuild("Secondnorth", "Thrall");
            }
        }
    }
}
