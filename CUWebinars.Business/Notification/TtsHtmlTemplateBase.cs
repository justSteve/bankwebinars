using CUWebinars.Html;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<T>:TemplateBase<T>
    {
        private TtsHtmlHelpers _ttsTemplateHelper;
        private TtsTemplateHelper _helper;

        public TtsHtmlHelpers TtsHtmlObj
        {
            get { return _ttsTemplateHelper ?? (_ttsTemplateHelper = new TtsHtmlHelpers()); }
        }

        public TtsTemplateHelper TtsTemplateHelperObject
        {
            get { return _helper ?? (_helper = new TtsTemplateHelper()); }
        }
    }
}
