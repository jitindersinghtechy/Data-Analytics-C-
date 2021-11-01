using System.Web;
using System.Web.Optimization;

namespace S2TAnalytics.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.pagepiling.min.js"
                        ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          //"~/Content/bootstrap.css",
            //          "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/angular.min.js",
                "~/Scripts/angular-validation.min.js",
                "~/Scripts/angular-validation-rule.min.js",
                "~/Scripts/angular-ui-router.js",
                "~/Scripts/angularjs-dropdown-multiselect.js",
                "~/App/Widgets/TopPerformerWidget/topPerformerWidgetApp.js",
                "~/App/app.js",
                "~/App/Widgets/TopPerformerWidget/topPerformerWidgetApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/highchart").Include(
                "~/Scripts/Highcharts/proj4.js",
                "~/Scripts/Highcharts/highcharts.js",
                "~/Scripts/Highcharts/highcharts-more.js",
                "~/Scripts/Highcharts/map.js",
                "~/Scripts/Highcharts/data.js",
                "~/Scripts/Highcharts/exporting.js",
                "~/Scripts/Highcharts/world.js",
                "~/Scripts/Highcharts/ae-all.js",
                "~/Scripts/Highcharts/at-all.js",
                "~/Scripts/Highcharts/au-all.js",
                "~/Scripts/Highcharts/br-all.js",
                "~/Scripts/Highcharts/ca-all.js",
                "~/Scripts/Highcharts/cn-all.js",
                "~/Scripts/Highcharts/de-all.js",
                "~/Scripts/Highcharts/fi-all.js",
                "~/Scripts/Highcharts/in-all.js",
                "~/Scripts/Highcharts/it-all.js",
                "~/Scripts/Highcharts/kw-all.js",
                "~/Scripts/Highcharts/my-all.js",
                "~/Scripts/Highcharts/ro-all.js",
                "~/Scripts/Highcharts/se-all.js",
                "~/Scripts/Highcharts/sg-all.js",
                "~/Scripts/Highcharts/sn-all.js",
                "~/Scripts/Highcharts/tr-all.js",
                "~/Scripts/Highcharts/ua-all.js",
                "~/Scripts/Highcharts/us-all.js",
                "~/Scripts/Highcharts/vn-all.js",
                "~/Scripts/Highcharts/id-all.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/services").Include(
                "~/App/Services/authenticationService.js",
                "~/App/Services/loginService.js",
                "~/App/Services/signUpService.js",
                "~/App/Services/emailConfirmService.js",
                "~/App/Services/dashboardService.js",
                "~/App/Services/performerService.js",
                "~/App/Services/instrumentService.js",
                "~/App/Services/performersService.js",
                "~/App/Services/topPerformersService.js",
                "~/App/Services/comparisonDataService.js",
                "~/App/Services/pinnedUsersService.js",
                "~/App/Services/topPinnedUsersService.js",
                "~/App/Services/userDetailsService.js",
                "~/App/Services/navMapService.js",
                "~/App/Services/topPerformersAndPinnedUsersService.js",
                "~/App/Services/configureService.js",
                "~/App/Services/compareService.js",
                "~/App/Services/setPasswordService.js",
                "~/App/Services/contactService.js",
                "~/App/Services/userAccountService.js",
                "~/App/Widgets/TopPerformerWidget/Services/topPerformersWidgetService.js",
                "~/App/Services/mainService.js"
                ));


            bundles.Add(new ScriptBundle("~/bundles/directives").Include(
                "~/App/Directives/preventDefault.js",
                "~/App/Directives/focusMe.js",
                "~/App/Directives/openCloseIcons.js",
                "~/App/Directives/script.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/controllers").Include(
                "~/App/Controllers/loginController.js",
                "~/App/Controllers/signUpController.js",
                "~/App/Controllers/emailConfirmController.js",
                "~/App/Controllers/dashboardController.js",
                "~/App/Controllers/performersController.js",
                "~/App/Controllers/pinnedUsersController.js",
                "~/App/Controllers/userDetailsController.js",
                "~/App/Controllers/mainController.js",
                "~/App/Controllers/configureController.js",
                "~/App/Controllers/compareController.js",
                 "~/App/Controllers/userAccountController.js",
                 "~/App/Controllers/setPasswordController.js",
                 "~/App/Controllers/contactController.js",
                 "~/App/Controllers/reportUserAccountController.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/components").Include(
               "~/App/Components/performersComponent.js",
               "~/App/Components/instrumentsLocationComponent.js",
               "~/App/Components/topPerformersComponent.js",
               "~/App/Components/comparisonDataComponent.js",
               "~/App/Components/pinnedUsersComponet.js",
               "~/App/Components/topPinnedUsersComponent.js",
               "~/App/Components/userDetailsComponent.js",
               "~/App/Components/instrumentsGroupComponent.js",
               "~/App/Components/navMapComponent.js",
               "~/App/Components/topPerformersAndPinnedUsersComponent.js",
               "~/App/Widgets/TopPerformerWidget/Components/topPerformersWidgetComponent.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/adminApp").Include(
              "~/Scripts/angular.min.js",
              "~/Scripts/angular-validation.min.js",
              "~/Scripts/angular-validation-rule.min.js",
              "~/Scripts/angular-ui-router.js",
              "~/Scripts/angularjs-dropdown-multiselect.js",
              "~/App/SuperAdmin/adminApp.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/adminServices").Include(
               "~/App/Services/authenticationService.js",
               "~/App/SuperAdmin/Services/adminDashboardService.js",
               "~/App/SuperAdmin/Services/adminLoginService.js",
               "~/App/SuperAdmin/Services/adminMainService.js",
               "~/App/SuperAdmin/Services/adminPlansService.js",
               "~/App/SuperAdmin/Services/adminSubscriberService.js",
               "~/App/SuperAdmin/Services/adminViewDetailsService.js",
               "~/App/SuperAdmin/Services/adminCouponService.js",
               "~/App/SuperAdmin/Services/adminNotificationsService.js"
               ));
            bundles.Add(new ScriptBundle("~/bundles/adminControllers").Include(
                "~/App/SuperAdmin/Controllers/adminDashboardController.js",
                "~/App/SuperAdmin/Controllers/adminLoginController.js",
                "~/App/SuperAdmin/Controllers/adminMainController.js",
                "~/App/SuperAdmin/Controllers/adminPlansController.js",
                "~/App/SuperAdmin/Controllers/adminSubscriberController.js",
                "~/App/SuperAdmin/Controllers/adminViewDetailsController.js",
                "~/App/SuperAdmin/Controllers/adminCouponController.js",
                "~/App/SuperAdmin/Controllers/adminNotificationsController.js"
               ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
