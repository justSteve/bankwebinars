using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CUWebinars.Web
{
    public class RssResult : SyndicationFeedResult<Rss20FeedFormatter>
    {
        public RssResult(SyndicationFeed feed)
            : base(feed)
        {
        }

        public override Rss20FeedFormatter ConstructFeedFormatter()
        {
            return new Rss20FeedFormatter(Feed);
        }

        public override XmlWriterSettings ConfigureXmlWriter()
        {
            return new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.None,
                Indent = true,
                Encoding = Encoding.UTF32,
                ConformanceLevel = ConformanceLevel.Document,
                OmitXmlDeclaration = true
            };
        }

        public override string ConfigureOutputContentType()
        {
            return "application/xml";
        }

        public override void PostProcessOutputBuffer(StringBuilder buffer)
        {
            var xmlDoc = XDocument.Parse(buffer.ToString());
            foreach (var element in xmlDoc.Descendants("channel").First().Descendants("item").Descendants("description"))
            {
                VerifyCdataHtmlEncoding(buffer, element);
            }

            foreach (var element in xmlDoc.Descendants("channel").First().Descendants("description"))
            {
                VerifyCdataHtmlEncoding(buffer, element);
            }

            buffer.Replace(" xmlns:a10=\"http://www.w3.org/2005/Atom\"", " xmlns:atom=\"http://www.w3.org/2005/Atom\"");
            buffer.Replace("a10:", "atom:");
        }

        private static void VerifyCdataHtmlEncoding(StringBuilder buffer, XElement element)
        {
            if (!element.Value.Contains("<") || !element.Value.Contains(">"))
            {
                return;
            }

            var cdataValue = string.Format("<{0}><![CDATA[{1}]]></{2}>", element.Name, element.Value, element.Name);
            buffer.Replace(element.ToString(), cdataValue);
        }
    }
}