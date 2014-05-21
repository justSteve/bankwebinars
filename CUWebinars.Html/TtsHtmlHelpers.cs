using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Html
{
    public class TtsHtmlHelpers
    {
        private const string ImageParameters = "{0} {1}";
        public static ControllerContext _controllerContext;
        private readonly HtmlHelper _htmlHelper;

        //public TtsHtmlHelpers(ControllerContext controllerContext)
        public TtsHtmlHelpers()
        {
            //_htmlHelper = new HtmlHelper(
            //    new ViewContext(
            //        _controllerContext,
            //        new WebFormView(_controllerContext, "omg"),
            //        new ViewDataDictionary(),
            //        new TempDataDictionary(),
            //        _controllerContext.HttpContext.Response.Output
            //        ),
            //    new ViewPage()
            //    );
        }

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
        
        public IHtmlString Link(string linkText, string src, object htmlAttributes = null)
        {
            var builder = new TagBuilder(UiConstants.Anchor);

            builder.InnerHtml = linkText;

            builder.MergeAttribute(UiConstants.SourceAttribute, src);

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
