using CUWebinars.Business.Core.Helpers;
using CUWebinars.Html;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<T>:TemplateBase<T>
    {
        private TtsHtmlHelpers _ttsTemplateHelper;

        public TtsHtmlHelpers TtsHtmlObj
        {
            get { return _ttsTemplateHelper ?? (_ttsTemplateHelper = new TtsHtmlHelpers()); }
        }
        
    }
}
