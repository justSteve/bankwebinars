using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Html
{
    public class TtsHtmlHelpers
    {
        private const string ImageParameters = "{0} {1}";

        public IHtmlString Image(string id, string url, string alternateText, object htmlAttributes = null)
        {
            // Instantiate a UrlHelper

            // Create tag builder
            var builder = new TagBuilder(UiConstants.ImageAttribute);

            // Create valid id
            if (!string.IsNullOrWhiteSpace(id))
            {
                builder.GenerateId(id);
            }

            // Add attributes
            builder.MergeAttribute(UiConstants.AltAttribute, alternateText);
            builder.MergeAttribute(UiConstants.SourceAttribute, url);


            if (!ReferenceEquals(null, htmlAttributes))
            {
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            // Render tag
            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public IHtmlString Label(string labelText, object htmlAttributes = null)
        {
            var builder = new TagBuilder(UiConstants.Label);

            builder.InnerHtml = labelText;

            if (!ReferenceEquals(null, htmlAttributes))
            {
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            // Render tag.
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

        public IHtmlString Span(string spanText, object htmlAttributes = null)
        {
            var builder = new TagBuilder(UiConstants.Span);

            builder.InnerHtml = spanText;

            if (!ReferenceEquals(null, htmlAttributes))
            {
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            // Render tag.
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }

    }
}
