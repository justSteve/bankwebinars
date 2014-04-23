using CUWebinars.Business.Models;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CUWebinars.Web.Core
{
    public static class CsvParseOps
    {
//PaymentStatus	AddtionaLocations	WebinarId	Registration Type	AffiliateId	FirstName	LastName	Institution	Title	Email	PhoneNumber	StreetAddress	StreetAddressLine2	City	State	ZipCode	Country

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
    }
}