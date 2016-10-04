using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Models
{
    public class MandrillIncomingMsg
    {
        public class Headers
        {
            public List<string> Received { get; set; }
            public string __invalid_name__Dkim_Signature { get; set; }
            public string __invalid_name__X_Google_Dkim_Signature { get; set; }
            public string __invalid_name__X_Gm_Message_State { get; set; }
            public string __invalid_name__Mime_Version { get; set; }
            public string __invalid_name__X_Received { get; set; }
            public string __invalid_name__In_Reply_To { get; set; }
            public string References { get; set; }
            public string Date { get; set; }
            public string __invalid_name__Message_Id { get; set; }
            public string Subject { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string __invalid_name__Content_Type { get; set; }
        }

        public class Spf
        {
            public string result { get; set; }
            public string detail { get; set; }
        }

        public class MatchedRule
        {
            public string name { get; set; }
            public double score { get; set; }
            public string description { get; set; }
        }

        public class SpamReport
        {
            public double score { get; set; }
            public List<MatchedRule> matched_rules { get; set; }
        }

        public class Dkim
        {
            public bool signed { get; set; }
            public bool valid { get; set; }
        }

        public class Msg
        {
            public string raw_msg { get; set; }
            public Headers headers { get; set; }
            public string text { get; set; }
            public bool text_flowed { get; set; }
            public string html { get; set; }
            public string from_email { get; set; }
            public string from_name { get; set; }
            public List<List<string>> to { get; set; }
            public string subject { get; set; }
            public Spf spf { get; set; }
            public SpamReport spam_report { get; set; }
            public Dkim dkim { get; set; }
            public string email { get; set; }
            public List<object> tags { get; set; }
            public object sender { get; set; }
            public object template { get; set; }
        }
        public class mandrill_events
        {
             
        public string @event { get; set; }
        public int ts { get; set; }
        public Msg msg { get; set; }
        }
    }
}