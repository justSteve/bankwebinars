using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Core.Helpers
{
   public static class StringExtensions
    {
        // as per http://stackoverflow.com/questions/444798/case-insensitive-containsstring
       // 
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        //public static bool Contains(this string source, string toCheck, StringComparison comp)
        //{
        //    return source.IndexOf(toCheck, comp) >= 0;
        //}

    }
}
