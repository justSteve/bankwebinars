/*** HtmlTemplateBase<> ***/
/*** Author: Abu Haider 
/*** September, 2011
/*** To be used as the Template base with the RazorEngine on CodePlex
/*** http://razorengine.codeplex.com
/*** Copyright 2011, Abu Haider, www.haiders.net
/*** Use at your own risk
/***/

using RazorEngine.Templating;

namespace CUWebinars.Business.Notification
{
    public class TtsHtmlTemplateBase<t>:TemplateBase<t>
    {
        private TtsTemplateHelper _ttsTemplateHelper;

        public TtsTemplateHelper TtsTemplateHelperObj
        {
            get { return _ttsTemplateHelper ?? (_ttsTemplateHelper = new TtsTemplateHelper()); }
        }
        
    }
}
