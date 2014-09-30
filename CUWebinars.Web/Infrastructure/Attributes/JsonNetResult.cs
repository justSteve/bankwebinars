using CUWebinars.Web.Infrastructure.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Text;
using System.Web.Mvc;

namespace CUWebinars.Web.Infrastructure.Attributes
{
    public class JsonNetResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public IsoDateTimeConverter IsoDateTimeConverter { get; set; }
        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (ReferenceEquals(null, SerializerSettings)) SerializerSettings = new JsonSerializerSettings();
            

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrWhiteSpace(ContentType)
              ? ContentType
              : "application/json";

            response.CacheControl = "no-cache";

            if (!ReferenceEquals(ContentEncoding, null)) 
                response.ContentEncoding = ContentEncoding;

            // Here we call the extension method Object.ToJsonNet().
            if (!ReferenceEquals(Data, null))
            {
                response.Write(!ReferenceEquals(IsoDateTimeConverter, null)
                    ? Data.ToJsonNet(SerializerSettings, Formatting, IsoDateTimeConverter)
                    : Data.ToJsonNet());
            }
        }
    }
}