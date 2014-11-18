using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<T>:TemplateBase<T>
    {
        //private TtsTemplateHelper _dateTimeHelper;
        private TtsConfigHelper _ttsConfigHelper;
        private TtsTemplateHelper _helper;

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
