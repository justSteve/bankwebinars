using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Web.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string JoinAsString(this IEnumerable<string> src, string separator = ",")
        {
            if (ReferenceEquals(src, null))
                return string.Empty;

            var enumerable = src as string[] ?? src.ToArray();

            return enumerable.Any() ? string.Join(separator, enumerable) : string.Empty;
        }
    }
}