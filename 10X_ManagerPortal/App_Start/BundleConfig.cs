using System.Web;
using System.Web.Optimization;

namespace _10X_ManagerPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Content/js/helpers.js",
                     "~/Content/assets/js/config.js",
                     "~/Content/js/menu.js",
                     "~/Content/assets/js/main.js",
                     "~/Content/js/extended-ui-perfect-scrollbar.js",
                     "~/Content/libs/popper/popper.js",
                     "~/Content/libs/apex-charts/apexcharts.js",
                     "~/Content/libs/sweetalert2/sweetalert2.js",
                     "~/Content/libs/select2/select2.js",
                     "~/Scripts/respond.js",
                     "~/Scripts/bootstrap.bundle.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/core.css",
                      "~/Content/assets/css/demo.css",
                      "~/Content/css/theme-raspberry-dark.css",
                      "~/Content/libs/perfect-scrollbar/perfect-scrollbar.css",
                      "~/Content/fonts/boxicons.css",
                      "~/Content/libs/sweetalert2/sweetalert2.css",
                      "~/Content/libs/i18n/i18n.css",
                      "~/Content/libs/tagify/tagify.css",
                      "~/Content/libs/typehead-js/typehead.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/ejscripts").Include(
                           "~/Scripts/jsrender.min.js",
                            "~/Scripts/ej/ej.web.all.min.js",
                            "~/Scripts/ej/ej.unobtrusive.min.js"));
            bundles.Add(new StyleBundle("~/bundles/ejstyles").Include(
                      "~/ejThemes/flat-saffron/ej.web.all.min.css"));
            BundleTable.EnableOptimizations = false;


        }
        //public static void RegisterBundles(BundleCollection bundles)
        //{
        //    bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
        //                "~/Scripts/jquery-{version}.js"));

        //    bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
        //                "~/Scripts/jquery.validate*"));

        //    // Use the development version of Modernizr to develop with and learn from. Then, when you're
        //    // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
        //    bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
        //                "~/Scripts/modernizr-*"));

        //    bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
        //              "~/Scripts/bootstrap.js"));

        //    bundles.Add(new StyleBundle("~/Content/css").Include(
        //              "~/Content/bootstrap.css",
        //              "~/Content/site.css"));
        //}
    }
}
