using CUWebinars.Html;
using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<T>:TemplateBase<T>
    {
        private TtsHtmlHelpers _ttsTemplateHelper;
        private TtsDateTimeTemplateHelper _dateTimeHelper;
        private TtsConfigHelper _ttsConfigHelper;
        private TtsTemplateHelper _helper;

        public TtsHtmlHelpers TtsHtmlObj
        {
            get { return _ttsTemplateHelper ?? (_ttsTemplateHelper = new TtsHtmlHelpers()); }
        }

        public TtsTemplateHelper TtsTemplateHelperObject
        {
            get { return _helper ?? (_helper = new TtsTemplateHelper()); }
        }

        public TtsConfigHelper TtsConfigHelperObject
        {
            get { return _ttsConfigHelper ?? (_ttsConfigHelper = new TtsConfigHelper()); }
        }
    }
}
