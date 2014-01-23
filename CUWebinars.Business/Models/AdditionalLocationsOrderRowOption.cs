using System;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Business.Models
{
    public class AdditionalLocationsOrderRowOption : OrderRowOption
    {
        protected string mEmailsString;
        private int _additionalLocationsCount;

        protected string EmailsString
        {
            get
            {
                if (Emails == null || Emails.Count == 0)
                    return null;

                return string.Join(",", Emails.Where(x => !string.IsNullOrEmpty(x)).ToArray());
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                Emails = new List<string>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public int AdditionalLocationsCount
        {
            get { return _additionalLocationsCount; }
            set { _additionalLocationsCount = value; }
        }

        public AdditionalLocationsOrderRowOption()
        {
            Emails = new List<string>();
        }

        public IList<string> Emails { get; set; }
    }
}
