using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core
{
    public static class CsvParseOps
    {
        private static readonly IDictionary<string, string> DataDictionary = new Dictionary<string, string>();

        public static IncomingOrderModel ParseCsvForIncomingOrderModel(Stream stream)
        {
            var incomingOrderModel = new IncomingOrderModel
            {
                BillingAddress = new Address(),
                ShippingAddress = new Address()
            };
            string line = string.Empty;
            string remainingData = string.Empty;

            using (var sr = new StreamReader(stream))
            {
                string[] keys, values;
                keys = values = null;

                line = sr.ReadLine();

                if (line != null)
                    keys = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                else
                    throw new NullReferenceException("The header section of the csv file is missing.");


                line = sr.ReadLine();
                if (line != null)
                {
                    values = line.Split(',');
                    DataDictionary.Clear();

                    for (int i = 0; i < keys.Length; i++)
                    {
                        DataDictionary.Add(keys[i].Trim(), values[i].Trim());
                    }
                }
                else
                    throw new NullReferenceException("The values section of the csv file is missing.");

                incomingOrderModel.Title = DataDictionary[keys[1]];
                incomingOrderModel.idWebinar = int.Parse(DataDictionary[keys[2]]);
                incomingOrderModel.idRegType = int.Parse(DataDictionary[keys[3]]);
                incomingOrderModel.idAffiliate = int.Parse(DataDictionary[keys[4]]);
                incomingOrderModel.FirstName = DataDictionary[keys[5]];
                incomingOrderModel.LastName = DataDictionary[keys[6]];
                incomingOrderModel.Institution = DataDictionary[keys[7]];
                incomingOrderModel.Title = DataDictionary[keys[8]]; ;
                incomingOrderModel.Email = DataDictionary[keys[9]];
                incomingOrderModel.BillingAddress.Phone = DataDictionary[keys[10]];
                incomingOrderModel.BillingAddress.StreetAddress = DataDictionary[keys[11]];
                incomingOrderModel.BillingAddress.StreetAddress2 = DataDictionary[keys[12]];
                incomingOrderModel.BillingAddress.City = DataDictionary[keys[13]];
                incomingOrderModel.BillingAddress.State = DataDictionary[keys[14]];
                incomingOrderModel.BillingAddress.Zip = DataDictionary[keys[15]];
                incomingOrderModel.BillingAddress.Country = DataDictionary[keys[16]];

                return incomingOrderModel;
            }

        }


        private static Address GetAddressFromData(string remainingData)
        {
            string addressData = remainingData.Substring(remainingData.IndexOf("{") + 1);
            addressData = addressData.Remove(addressData.IndexOf("}"));

            var keyValuePairs = GetKeyValuePairs(addressData);

            return new Address
            {
                AddressType = keyValuePairs["AddressType"],
                Name = keyValuePairs["Name"],
                Phone = keyValuePairs["Phone"],
                StreetAddress = keyValuePairs["StreetAddress"],
                StreetAddress2 = keyValuePairs["StreetAddress2"],
                City = keyValuePairs["City"],
                Zip = keyValuePairs["Zip"],
                State = keyValuePairs["State"],
                Country = keyValuePairs["Country"],
            };
        }

        private static Dictionary<string, string> GetKeyValuePairs(string rawData)
        {
            string[] fields = rawData.Split(',');
            var keyValuePairs = new Dictionary<string, string>(fields.Count());

            foreach (var field in fields)
            {
                keyValuePairs.Add(field.Substring(0, field.IndexOf(":")), field.Substring(field.IndexOf(":") + 1));
            }

            return keyValuePairs;
        }

        private static IEnumerable<IncomingAdditionalLocation> GetAdditionaLocationsFromData(string allData)
        {
            string locationsRaw = allData.Substring(0, allData.IndexOf(']'));
            locationsRaw = locationsRaw.Substring(locationsRaw.IndexOf('{') + 1);
            locationsRaw = locationsRaw.Remove(locationsRaw.LastIndexOf("}"));

            string[] locationsAsObjects = locationsRaw.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var locationAsObject in locationsAsObjects)
            {
                var keyValuePairs = GetKeyValuePairs(locationAsObject);

                yield return
                    new IncomingAdditionalLocation
                    {
                        Email = keyValuePairs["Email"],
                        FirstName = keyValuePairs["FirstName"],
                        LastName = keyValuePairs["LastName"]
                    };
            }
        }
    }

    public static class StringExtensions
    {
        public const string NotFound = "__NOT_FOUND__";
        public const string PathParameter = "path";

        /// <summary>
        /// Find the index of the nth char in a string. For example, an UNC may have a long path with 7 slashes. You may want to find the 3rd slash (from the left).
        /// </summary>
        /// <param name="path"></param>
        /// <param name="number"></param>
        /// <param name="delimiter"></param>
        /// <returns>The index of the nth char in a string.</returns>
        public static int GetIndexOfNthChar(this string path, int number, char delimiter)
        {
            if (ReferenceEquals(path, null))
                throw new ArgumentNullException(string.Format("The \"{0}\" parameter cannot be null.", PathParameter), PathParameter);
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(string.Format("The \"{0}\" parameter cannot be empty of composed only of white space.", PathParameter), PathParameter);

            return GetIndexOfNthCharHelper(path, number, 0, delimiter);
        }

        /// <summary>
        /// Recursive helper method for GetIndexOfNthChar.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="number"></param>
        /// <param name="runningCount">This really needs to have a value of 0.</param>
        /// <param name="delimiter"></param>
        /// <returns>The index of the nth char in a string.</returns>
        private static int GetIndexOfNthCharHelper(string path, int number, int runningCount, char delimiter)
        {
            int currentIndexOfDelimiter = path.IndexOf(delimiter);

            if (currentIndexOfDelimiter == -1)
                throw new ArgumentOutOfRangeException(string.Format("You have requested the position of the Nth char where there are less than N instances of the char in the string."));

            if (number == 0)
                return currentIndexOfDelimiter + runningCount;

            runningCount += currentIndexOfDelimiter + 1;

            return GetIndexOfNthCharHelper(path.Substring(currentIndexOfDelimiter + 1), number - 1, runningCount, delimiter);
        }

        /// <summary>
        /// Finds the substring from the nth char to the mth char. Unlike SubstringBetweenNthAndMth, the string returned includes the chars at the Nth and Mth potsitions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>The substring from the nth char to the mth char.</returns>
        public static string SubstringFromNthToMth(this string source, int first, int second)
        {
            if (string.IsNullOrEmpty(source))
                return NotFound;

            if (second < first)
                throw new ArgumentOutOfRangeException("second", "The second index cannot be less than the first");
            if (first < 0)
                throw new ArgumentOutOfRangeException("first", "The first index cannot be less than 0");


            int count = second - first + 1;

            return source.Substring(first, count);
        }

    }
}