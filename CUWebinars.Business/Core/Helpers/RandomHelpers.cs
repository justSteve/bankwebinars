using System;
using System.Security.Cryptography;
using System.Text;

namespace CUWebinars.Business.Core.Helpers
{
    public class RandomHelpers
    {
        public static string GenerateRandomCode(int size)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var randomBytes = new byte[size];

            rngCryptoServiceProvider.GetNonZeroBytes(randomBytes);


            var builder = new StringBuilder(size);

            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * NextDouble(rngCryptoServiceProvider) + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        internal static double NextDouble(RandomNumberGenerator rngCryptoServiceProvider)
        {
            var buffer = new byte[4];

            rngCryptoServiceProvider.GetBytes(buffer);

            int result = BitConverter.ToInt32(buffer, 0);
            return new Random(result).NextDouble();
        }
    }
}
