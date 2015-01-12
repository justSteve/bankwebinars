using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Replace invalid characters in a string with empty strings. 
        /// </summary>
        /// <param name="stringToClean">Type: System.String. The string to parse for illegal characters.</param>
        /// <returns>Type: System.String. A string stripped of the illegal characters.</returns>
        public static string RemoveIllegalCharacters(this string stringToClean)
        {
            if (stringToClean == null) throw new ArgumentNullException("stringToClean");
            try
            {
                return Regex.Replace(
                    stringToClean,
                    @"[<>:\""/\\|?*,]", // These are illegal for file/directory naming purposes in Windows.
                    string.Empty,
                    RegexOptions.None,
                    TimeSpan.FromSeconds(1.5)
                    );
            }
            catch (RegexMatchTimeoutException)
            {
                // If we timeout when replacing invalid characters,  
                // we should return Empty. 
                return string.Empty;
            }
        }

        /// <summary>
        /// Replace invalid characters in a string with empty strings. 
        /// </summary>
        /// <param name="stringToClean">Type: System.String. The string to parse for illegal characters.</param>
        /// <returns>Type: System.String. A string stripped of the illegal characters.</returns>
        public static string ReplaceSpacesWithHyphens(this string stringToTransform)
        {
            if (stringToTransform == null) throw new ArgumentNullException("stringToTransform");
            try
            {
                return stringToTransform.Replace(' ', '-');
            }
            catch (Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return string.Empty;
            }
        }

        /// <summary>
        /// This simple string extension is based on the VB6 "Right" function which returns a certain number of characters, counting from the right.
        /// </summary>
        public static string SubstringFromRight(this string source, int numberOfChars)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            if (source.Length > numberOfChars)
            {
                return source.Substring(source.Length - numberOfChars, numberOfChars);
            }

            throw new ArgumentOutOfRangeException("numberOfChars",
                string.Format("The string \"{0}\" has a length which is less than {1} characters.", source, numberOfChars));
        }
    }
}