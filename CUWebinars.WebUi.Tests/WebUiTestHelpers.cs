using System;
using System.Text;

namespace CUWebinars.WebUi.Tests
{
    internal class WebUiTestHelpers
    {
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// Taken from StackOverflow answer http://stackoverflow.com/a/1122519/540156
        /// </summary>
        internal static string RandomString(int size)
        {
            var builder = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
