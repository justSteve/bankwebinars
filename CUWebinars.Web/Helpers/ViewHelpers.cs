using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
using System.Text;
using System.Web.Mvc;
using log4net.Util.TypeConverters;

// from here: http://www.codemag.com/article/1312081
namespace CUWebinars.Web.Helpers
{
    public class ViewHelpers
    {

        public static TagBuilder GetRendererOfAdditionalLocations(IList<string> emailAddresses)
        {
            const string locationsSpanPrefix = "LocationSpan-";
            const string breakSuffix = "-break";
            const string additionalLocationDeleteSuffix = "-AdditionLocationEmail-delete";
            const string additionalLocationEmailPrefix = "AdditionalLocationEmail-";
            const string nonBreakingSpace = "&nbsp;";
            var stringBuilder = new StringBuilder();


            var spanBuilder = new TagBuilder("span");
            var inputBuilder = new TagBuilder("input");
            var iconBuilder = new TagBuilder("i");

            /***************** The generataed element will look like this: *****************/
            /*
                <span id="LocationSpan-0">
                   <input aria-describedby="AdditionalLocationEmail_0-error" aria-invalid="false" class="valid" id="AdditionalLocationEmail-0" name="AdditionalLocations[0].Email" placeholder="Enter email address" type="email" value="test@yahoo.com">&nbsp;
                   <i class="icon-white icon-trash" id="0-AdditionLocationEmail-delete" style="cursor: pointer"></i>
                   <br id="0-break">
                </span>             
             */

            if (emailAddresses.Any())
            {
                for (var i = 0; i < emailAddresses.Count; i++)
                {
                    spanBuilder = new TagBuilder("span");
                    inputBuilder = new TagBuilder("input");
                    iconBuilder = new TagBuilder("i");

                    inputBuilder.GenerateId(additionalLocationEmailPrefix + i);
                    inputBuilder.MergeAttributes(new Dictionary<string, string>
                    {
                        {"name", "AdditionalLocations[" + i + "].Email"},
                        {"type", "email"},
                        {"placeholder", "Enter email address"},
                        {"aria-invalid", "false"},
                        {"aria-describedby", "AdditionalLocationEmail_" + i + "-error"},
                        {"value", emailAddresses[i]},
                    });
                    inputBuilder.AddCssClass("valid");

                    iconBuilder.AddCssClass("icon-trash");
                    iconBuilder.AddCssClass("icon-white");
                    iconBuilder.MergeAttribute("style", "cursor: pointer");
                    iconBuilder.MergeAttribute("id", string.Concat(i, additionalLocationDeleteSuffix));

                    spanBuilder.GenerateId(locationsSpanPrefix + i);
                    spanBuilder.InnerHtml = string.Concat(
                        inputBuilder.ToString(TagRenderMode.SelfClosing),
                        nonBreakingSpace,
                        iconBuilder.ToString(TagRenderMode.Normal),
                        "<br id=" + i + breakSuffix + ">"
                        );

                    stringBuilder.Append(spanBuilder.ToString(TagRenderMode.Normal));
                }
            }
            //else
            //{
            //    spanBuilder = new TagBuilder("span");
            //    spanBuilder.GenerateId("noLocationsText");
            //    spanBuilder.InnerHtml = "No additionalLocations yet";
            //    spanBuilder.AddCssClass("text");
            //    spanBuilder.AddCssClass("text-info");
            //}


            var div = new TagBuilder("div");
            div.GenerateId("collectAdditionalLocations");
            div.AddCssClass("addLocsBox");
            div.InnerHtml = emailAddresses.Any() ? stringBuilder.ToString() : spanBuilder.ToString(TagRenderMode.Normal);

            return div;

        }

        public static string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null ||
                viewEngineResult.View == null) // a.s. fixed bug here, viewEngineResult was not null but viewEngineResult.View was
                throw new System.IO.FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new System.IO.StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public static string RenderDiscountCaption(Discount discount)
        {
            var sb = new StringBuilder();
            if (!ReferenceEquals(null, discount))
            {
                if (discount.DiscountType == DiscountType.Compensation)
                {
                    int switchOn = Convert.ToInt32(discount.CreditsRemain);
                    if (switchOn > 1) switchOn = 2;
                    if (switchOn < 1) switchOn = 0;

                    string discOff;
                    switch (switchOn)
                    {
                        case 0:
                            if (discount.FlatOff > 0)
                            {
                                discOff = "$" + discount.FlatOff;
                            }
                            else
                            {
                                discOff = discount.PercentOff + "%";
                            }

                            sb.AppendFormat("class=\"text-error\">Your discount was redeemed for {0} the registration.</span>", discOff);
                            break;                        
                        case 1:
                            if (discount.FlatOff > 0)
                            {
                                discOff = "$" + discount.FlatOff;
                            }
                            else
                            {
                                discOff = discount.PercentOff + "%";
                            }

                            sb.AppendFormat("<span class=\"text-success\">You have a discount of {0} available.</span>", discOff);
                            break;
                 
                        case 2:
                            if (discount.FlatOff > 0)
                            {
                                discOff = "$" + discount.FlatOff;
                            }
                            else
                            {
                                discOff = discount.PercentOff + "%";
                            }

                            sb.AppendFormat("<span class=\"text-success\">You have discounts of {0} off your next {1} registrations.</span>", discOff, discount.CreditsRemain);
                            break;
                    }
                }
                if (discount.DiscountType == DiscountType.Subscription)
                {
                    sb.AppendFormat("<div  style=\"font-size: small\" class=\"text-success\">Your subscription ({0}) has {1} credits remaining.</div>", discount.DiscountCode, discount.CreditsRemain.ToString().Replace(".00", ""));
                }

            }
            return sb.ToString();
        }
    }
}