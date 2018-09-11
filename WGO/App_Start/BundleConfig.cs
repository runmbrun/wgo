﻿using System.Web;
using System.Web.Optimization;

namespace WGO
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        //"~/Scripts/jquery-{version}.slim.min.js"));
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryblock").Include(
                        "~/Scripts/jquery.blockUI*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      //"~/Scripts/popper.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/popperUMD.min.js",
                      //"~/Scripts/bootstrap.js",
                      //"~/Scripts/bootstrap.bundle.js",                      
                      //"~/Scripts/popper-utils.js",
                      //"~/Scripts/respond.min.js"));
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      //"~/Scripts/bootstrap-grid.css",
                      //"~/Scripts/bootstrap-reboot.css",
                      "~/Content/site.css"));
        }
    }
}
