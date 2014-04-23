using CUWebinars.Business.Models;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebGrease.Css.Extensions;

namespace CUWebinars.Web.Core
{
    public static class CsvParseOps
    {
//PaymentStatus	AddtionaLocations	WebinarId	Registration Type	AffiliateId	FirstName	LastName	Institution	Title	Email	PhoneNumber	StreetAddress	StreetAddressLine2	City	State	ZipCode	Country
// some csv | json resources:
        //http://www.codeproject.com/Articles/14254/Converting-CSV-Data-to-Objects
        //http://www.codeproject.com/Articles/9258/A-Fast-CSV-Reader
        //http://stackoverflow.com/questions/3268622/regex-to-split-line-csv-file
        //http://staceyw1.wordpress.com/2008/11/13/back-to-csv-convert-csv-text-to-objects-via-json/

    public static IList<IncomingOrderModel> ParseCsvForIncomingOrderModel(Stream stream)
        {
            IList<IncomingOrderModel> incomingOrderModels = new List<IncomingOrderModel>();

            string line = string.Empty;

            using (var sr = new StreamReader(stream))
            {
                string[] keys, values;
                keys = values = null;

                line = sr.ReadLine();

                if (line != null)
                    keys = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                else
                    throw new NullReferenceException("The header section of the csv file is missing.");


                while((line = sr.ReadLine()) != null)
                {
                    var incomingOrderModel = new IncomingOrderModel
                    {
                        BillingAddress = new Address(),
                        ShippingAddress = new Address()
                    };
                    var dataDictionary = new Dictionary<string, string>(keys.Length);

                    values = line.Split(',');

                    for (int i = 0; i < keys.Length; i++)
                    {
                        dataDictionary.Add(keys[i].Trim(), values[i].Trim());
                    }

                    incomingOrderModel.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), dataDictionary[keys[0]], true);

                    //  Additional Locations are at index 1
                    var addresses = dataDictionary[keys[1]].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var additionalLocations = new List<IncomingAdditionalLocation>(addresses.Length);
                    additionalLocations.AddRange(addresses.Select(address => new IncomingAdditionalLocation {Email = address}));
                    incomingOrderModel.AdditionalLocation = additionalLocations;

                    incomingOrderModel.idWebinar = int.Parse(dataDictionary[keys[2]]);
                    incomingOrderModel.idRegType = int.Parse(dataDictionary[keys[3]]);
                    incomingOrderModel.idAffiliate = int.Parse(dataDictionary[keys[4]]);
                    incomingOrderModel.FirstName = dataDictionary[keys[5]];
                    incomingOrderModel.LastName = dataDictionary[keys[6]];
                    incomingOrderModel.Institution = dataDictionary[keys[7]];
                    incomingOrderModel.Title = dataDictionary[keys[8]]; ;
                    incomingOrderModel.Email = dataDictionary[keys[9]];
                    incomingOrderModel.BillingAddress.Name = string.Concat(incomingOrderModel.FirstName, " ", incomingOrderModel.LastName);
                    incomingOrderModel.BillingAddress.Phone = dataDictionary[keys[10]];
                    incomingOrderModel.BillingAddress.StreetAddress = dataDictionary[keys[11]];
                    incomingOrderModel.BillingAddress.StreetAddress2 = dataDictionary[keys[12]];
                    incomingOrderModel.BillingAddress.City = dataDictionary[keys[13]];
                    incomingOrderModel.BillingAddress.State = dataDictionary[keys[14]];
                    incomingOrderModel.BillingAddress.Zip = dataDictionary[keys[15]];
                    incomingOrderModel.BillingAddress.Country = dataDictionary[keys[16]];
                    incomingOrderModel.BillingAddress.AddressType = WebUiConstants.BillingAddress;

                    //  If there is a name for the shipping address, we need to parse the address
                    var name = dataDictionary[keys[17]];

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        incomingOrderModels.Add(incomingOrderModel);
                    }
                    else
                    {
                        incomingOrderModel.ShippingAddress.Name = name;
                        incomingOrderModel.ShippingAddress.Phone = dataDictionary[keys[18]];
                        incomingOrderModel.ShippingAddress.StreetAddress = dataDictionary[keys[19]];
                        incomingOrderModel.ShippingAddress.StreetAddress2 = dataDictionary[keys[20]];
                        incomingOrderModel.ShippingAddress.City = dataDictionary[keys[21]];
                        incomingOrderModel.ShippingAddress.State = dataDictionary[keys[22]];
                        incomingOrderModel.ShippingAddress.Zip = dataDictionary[keys[23]];
                        incomingOrderModel.ShippingAddress.Country = dataDictionary[keys[24]];
                        incomingOrderModel.ShippingAddress.AddressType = WebUiConstants.ShippingAddress;
                    }
                }

                return incomingOrderModels;
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