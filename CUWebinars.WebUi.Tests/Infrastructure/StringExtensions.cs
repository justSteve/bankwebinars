using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.WebUi.Tests.Infrastructure
{
    public static class StringExtensions
    {
        public const string NotFound = "__NOT_FOUND__";

        /// <summary>
        /// Finds a substring in a string where that substring "starts with" a certain string. 
        /// This reduces the code you would otherwise have to write to perform this function.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns>The substring within the source string.</returns>
        public static string SubstringFrom(this string source, string start, int? count = null)
        {
            if (string.IsNullOrEmpty(source))
                return NotFound;

            if (string.IsNullOrEmpty(start))
                throw new ArgumentException(string.Format("The \"{0}\" parameter cannot be null or an empty string.", "start"), "start");

            int indexOfString = source.IndexOf(start, StringComparison.Ordinal);

            //  If the string is not contained in the source string at all, return the NotFound constant. The caller can guard against it.
            if (indexOfString < 0)
                return NotFound;

            if (!count.HasValue)
            {
                return source.Substring(indexOfString);
            }

            if (count.Value > 0)
            {
                return source.Substring(indexOfString, count.Value);
            }

            throw new Exception(string.Format("If the \"{0}\" parameter is not null, it must be greated than 0.", "count"));
        }
    }
}
