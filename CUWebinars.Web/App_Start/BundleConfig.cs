using System.Web.Optimization;
using CUWebinars.Web.Core;

namespace CUWebinars.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725

        public static readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;

        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(

                //"~/Scripts/jquery-1.9.1.js"
                "~/Scripts/jquery-1.11.1.js"
                //"~/Scripts/jquery-1.8.2.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.11.1.js"));
            //"~/Scripts/jquery-ui-1.10.3.custom.js"));
            //"~/Scripts/jquery-ui-1.8.24.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*")
                        );

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                        "~/Scripts/toastr.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/AdditionalLocation").Include(
                "~/Scripts/app/cart/add-additional-locations.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/add-password").Include(
                "~/Scripts/app/cart/add-password.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/manage-webinar-files").Include(
                "~/Scripts/app/cart/update-connection-info.js",
                "~/Scripts/app/cart/update-recording.js",
                "~/Scripts/app/cart/update-webinar-files.js"
                ));

            
            bundles.Add(new ScriptBundle("~/bundles/add-quiz").Include(
                "~/Scripts/app/toastLogger.js",
                "~/Scripts/app/quiz/quiz.js",
                "~/Scripts/app/admin/add-quiz.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/do-quiz").Include(
                "~/Scripts/app/toastLogger.js",
                "~/Scripts/app/quiz/quiz.js",
                "~/Scripts/app/quiz/do-quiz.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/edit-quiz").Include(
                "~/Scripts/app/toastLogger.js",
                "~/Scripts/app/quiz/quiz.js",
                "~/Scripts/app/quiz/edit-quiz.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/manage-order-from-details").Include(
                "~/Scripts/toastr.js",
                "~/Scripts/app/toastLogger.js",
                "~/Scripts/app/admin/manage-order-from-details.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/manage-claims").Include(
                "~/Scripts/app/admin/claims-management.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/click-to-join").Include(
                "~/Scripts/app/admin/click-to-join.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/manage-webinar-from-details").Include(
                "~/Scripts/cleditor/jquery.cleditor.min.js",
                "~/Scripts/cleditor/jquery.cleditor.xhtml.min.js",
                "~/Scripts/app/webinar/manage-webinar-from-details.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/CommonModules").Include(
                "~/Scripts/app/constants.js",
                "~/Scripts/app/extensions.js",
                "~/Scripts/underscore.js",
                "~/Scripts/purl.js", // excellent lib for parsing and working with the address bar content i.e. urls
                "~/Scripts/app/utilities.js",
                "~/Scripts/app/form-processor.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/myApp").Include(
                //TODO: devise bundles such that the Public layout calls BS2.3x while AdminLayout calls 3x
                        //"~/Scripts/bootstrap3/bootstrap.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap-modal.js",
                        "~/Scripts/bootstrap-modalmanager.js",
                        "~/Content/js/jquery-plugins/dataTables/media/js/jquery.dataTables.js",
                        "~/Content/js/jquery-plugins/dataTables/plugins/fnSetFilteringDelay.js"
                //"~/Scripts/app/cart/additional-locations.js"
                //,"~/Scripts/app/cart/edit-order.js"
                        ));
            
            bundles.Add(new ScriptBundle("~/bundles/create-order").Include(
                //"~/Scripts/app/cart/additional-locations.js",
                         "~/Scripts/app/cart/create-order-new.js",
                         "~/Scripts/app/cart/register-during-checkout.js",
                         "~/Scripts/app/cart/details.js",
                         "~/Scripts/app/cart/register-user-in-cart.js"
                //,"~/Scripts/app/cart/edit-order.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/create-order-affiliate").Include(
                         "~/Scripts/app/cart/create-order-new.js",
                //"~/Scripts/app/cart/register-during-checkout.js", // required because some functions here are common to all cart operations
                         "~/Scripts/app/cart/details-affiliate.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/CrispApp").Include(
                        "~/Content/js/ddsmoothmenu-min.js",         //desktop edge detect menu --> 
                        "~/Content/js/jquery.dcjqaccordion.2.7.min.js",         //mobile multi-level accordion menu --> 
                        "~/Content/js/jquery.easytabs.min.js",         //tabs/testimonials --> 
                        "~/Content/js/slide-to-top-accordion-min.js",         //slide to top accordion toggle --> 
                        "~/Content/js/jquery.easing-1.3.min.js", //easing--> 
                        "~/Content/js/jquery.flexslider-min.js",  //https://github.com/woothemes/FlexSlider/issues?state=open//flexslider content slider twitter slider and initializations--> 
                        "~/Content/js/responsive-tables.js",  //responsive table--> 
                        "~/Content/js/jquery.fitvid.js",         //responsive videos --> 
                //initialize scripts / custom scripts all pages--> 
                        "~/Content/js/scripts.js"
                //"~/Scripts/app/waitButton.js",
                //"~/Scripts/app/EditOrder.js"
                        ));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/app-certificate").Include(
                        "~/Scripts/app/constants.js",
                        "~/Scripts/app/common.js",
                        "~/Scripts/app/web-user-certificate-manage.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/app-processing-manage").Include(
                        "~/Scripts/app/constants.js",
                        "~/Scripts/app/common.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/web-user-details-manage").Include(
                        "~/Scripts/app/web-user-details-manage.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/reset-password-form").Include(
                        "~/Scripts/app/constants.js",
                        "~/Scripts/app/passwordReset/password-reset-submitter.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/app-processing-register").Include(
                        "~/Scripts/app/constants.js",
                        "~/Scripts/app/common.js",
                        "~/Scripts/app/register/register-user.js",
                        "~/Scripts/app/register/index.js",
                        "~/Scripts/app/passwordReset/password-reset.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/membership-notifications-ops").Include(
                "~/Scripts/app/membership-notifications.js",
                "~/Scripts/app/web-user-details-register.js"
            ));
            
            bundles.Add(new ScriptBundle("~/bundles/identify").Include(
                "~/Scripts/app/webinar/identify.js"
            ));

            bundles.Add(new StyleBundle("~/Content/create-order-details").Include(
                "~/Content/details.css"
                ));

            bundles.Add(new StyleBundle("~/Content/webinar-ops").Include(
                "~/Content/webinar.css"
                ));

            bundles.Add(new StyleBundle("~/Content/add-quiz").Include(
                "~/Content/quiz.css"
                ));

            bundles.Add(new StyleBundle("~/Content/notifications-ops-styles").Include(
                "~/Content/toastr.css",
                "~/Content/notification-ops.css"
                ));

            bundles.Add(new StyleBundle("~/Content/manage-order-styles").Include(
                "~/Content/toastr.css",
                "~/Content/manage-order.css"
                ));

            bundles.Add(new StyleBundle("~/Content/register-user-styles").Include(
                "~/Content/css/create-user-form.css"
                ));

            bundles.Add(new StyleBundle("~/Content/CrispCSS").Include(
                "~/Content/css/styleCrisp.css",
                "~/Content/css/header-1.css",
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-responsive.css",
//                "~/Scripts/bootstrap3/css/bootstrap.css",
                "~/Content/css/html-content-slider-flexslider.css",
                "~/Scripts/AutoComplete/css/styles.css",
                "~/Scripts/AutoComplete/css/styles.css/ui-lightness/jquery-ui-1.10.3.custom.css",
                "~/Content/validation.css"
                ));

            bundles.Add(new StyleBundle("~/Content/quiz-bootstrap").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/bootstrap-responsive.css",
                "~/Content/css/font-awesome.css")
                );


            bundles.Add(new StyleBundle("~/Content/common-styles").Include(
                "~/Content/common.css")
                );

            bundles.Add(new StyleBundle("~/Content/claims-management").Include(
                "~/Content/claims-mment.css")
                );

            bundles.Add(new StyleBundle("~/Content/quiz-general").Include(
                "~/Content/do-quiz.css")
                );

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            //BundleTable.EnableOptimizations = true;
        }
    }
}