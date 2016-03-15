using System;

namespace CUWebinars.Web.Core
{
    public class CalendarRssDTO
    {
        public int id { get; set; }
        public string Title { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public string Language { get; set; }
        public string PublishedDate { get; set; }
        public string Content { get; set; }
        public DateTime EventDate { get; set; }
    }
}