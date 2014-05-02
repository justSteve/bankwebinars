using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Html
{
    public sealed class UiConstants
    {
        public const string Action = "action";
        public const string Controller = "controller";
        public const string JsonMimeType = "application/json";


        //  HTML Related
        public const string AltAttribute = "alt";
        public const string Anchor = "a";
        public const string Button = "button";
        public const string Href = "href";
        public const string Image = "image";
        public const string ImageAttribute = "img";
        public const string InputAttribute = "input";
        public const string JavascriptTypeText = "text/javascript";
        public const string Label = "label";
        public const string SourceAttribute = "src";
        public const string ScriptAttribute = "script";
        public const string ScriptsDirectory = "~/Scripts/";
        public const string Submit = "submit";
        public const string TypeAttribute = "type";

        //  Views
        public const string PageNotFound = "PageNotFound";
        public const string StaticContent = "StaticContent";

        //  Validation/Messages
        public const string FutureDateValidationMsg = "Date cannot be a future date";
        public const string NoFutureDates = "nofuturedates";
        public const string NoSessionStateExceptionMsg = "NoSessionState";
        public const string RequiredDateValidationMsg = "This field must be a valid date";
        public const string ServerErrorPage = "ServerError";
        public const string SessionNotAvailable = "SessionNotAvailable";
        public const string ValidDate = "validdate";
    }
}
