using System;
using System.Collections.Generic;
using CUWebinars.Business.Core.Extensions;

namespace CUWebinars.Business.Core.Helpers
{
    public class EventHandlerHelpers
    {
        public static IList<string> GetCcEmailAddresses(string addresses)
        {
            IList<string> ccEmailAddresses = new List<string>();

            //  assume user has separated email addresses with either a semi-colon or a comma, as is convention.
            ccEmailAddresses.AddRange(addresses.Contains(";")
                ? addresses.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                : addresses.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            return ccEmailAddresses;
        }

    }
}
