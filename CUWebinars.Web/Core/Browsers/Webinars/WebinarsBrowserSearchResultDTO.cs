using System.Collections.Generic;

namespace CUWebinars.Web.Core.Browsers.Webinars
{
    public class WebinarsBrowserSearchResultDTO
    {
        public int TotalCount { get; set; }
        public int FoundCount { get; set; }

        private IList<WebinarsBrowserSearchResultEntryDTO> _entries = new List<WebinarsBrowserSearchResultEntryDTO>();
        public IList<WebinarsBrowserSearchResultEntryDTO> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }
    }
}