using CUWebinars.Html;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<T>:TemplateBase<T>
    {
        private TtsHtmlHelpers _ttsTemplateHelper;
        private TtsDateTimeTemplateHelper _dateTimeHelper;

        public TtsHtmlHelpers TtsHtmlObj
        {
            get { return _ttsTemplateHelper ?? (_ttsTemplateHelper = new TtsHtmlHelpers()); }
        }

        public TtsDateTimeTemplateHelper TtsDateTimeTemplateHelperObject
        {
            get { return _dateTimeHelper ?? (_dateTimeHelper = new TtsDateTimeTemplateHelper()); }
        }
    }
}
