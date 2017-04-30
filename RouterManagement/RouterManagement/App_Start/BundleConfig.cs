using System.Web.Optimization;

namespace RouterManagement
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Standard libraries and styles

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/PagesStyles/site.css"));

            #endregion

            #region Additional plugins and styles

            bundles.Add(new ScriptBundle("~/bundles/ListJS").Include(
                        "~/Scripts/list.js"));

            bundles.Add(new ScriptBundle("~/bundles/Spin").Include(
                "~/Scripts/jquery.spin.js"));

            bundles.Add(new StyleBundle("~/Content/Spin").Include(
                "~/Content/jquery.spin.css"));

            bundles.Add(new ScriptBundle("~/bundles/Notify").Include(
                "~/Scripts/bootstrap-notify.js"));

            #endregion

            #region Pages Styles

            #endregion
        }
    }
}
