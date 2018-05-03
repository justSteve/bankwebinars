
using System.Configuration;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using CUWebinars.Business.Services;
using System.Security.Cryptography;
using System.Text;

namespace CUWebinars.Business.Core.Helpers
{
    public class RandomHelpers
    {
        static Random random = new Random();

        /// <summary>
        /// Taken from this Stackoverflow answer: http://stackoverflow.com/a/1344255/540156
        /// Lowercase letters removed and size of array adjusted accordingly.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string GetUniqueCode(int maxSize, string field)
        {
            char[] chars = "abcdefghijkmnopqrstwvxyz234567890".ToCharArray();

            byte[] data = null;

            using (var crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[random.Next(4, maxSize)];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            var isUnique = dataOperations.CheckForUnique(result.ToString(), field);

            if (isUnique != "isUnique")
                GetUniqueCode(4, field);
            return result.ToString();
        }

        /// <summary>
        /// Taken from this Stackoverflow answer: http://stackoverflow.com/a/1344255/540156
        /// Lowercase letters removed and size of array adjusted accordingly.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static string GetUniqueCode(int maxSize)
        {
            char[] chars = "abcdefghijkmnopqrstwvxyz234567890".ToCharArray();

            byte[] data = null;

            using (var crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[random.Next(4, maxSize)];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(maxSize);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static string GenerateRandomCode(int size)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();

            var randomBytes = new byte[size];

            rngCryptoServiceProvider.GetNonZeroBytes(randomBytes);


            var builder = new StringBuilder(size);

            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * NextDouble(rngCryptoServiceProvider) + 48)));
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
