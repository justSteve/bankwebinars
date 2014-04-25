using CUWebinars.Business.Models;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

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

            using (var textFieldParser = new TextFieldParser(stream))
            {
                textFieldParser.TextFieldType = FieldType.Delimited;
                textFieldParser.SetDelimiters(",");
                textFieldParser.HasFieldsEnclosedInQuotes = false;
                textFieldParser.CommentTokens = new[] { "PaymentStatus" }; //  Discard header - PaymentStatus is first cell at A,1

                var lines = new List<string[]>();


                while(!textFieldParser.EndOfData)
                {
                    var lineAsSeparatedFields = textFieldParser.ReadFields();

                    //  First field is mandatory. If blank, the whole line must be blank.
                    if (string.IsNullOrWhiteSpace(lineAsSeparatedFields[0])) continue;

                    lines.Add(lineAsSeparatedFields);
                }

                 lines.ForEach((fields) =>
                {
                    var incomingOrderModel = new IncomingOrderModel
                    {
                        BillingAddress = new Address(),
                        ShippingAddress = new Address()
                    };

                    incomingOrderModel.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), fields[0], true);

                    //  Additional Locations are at index 1
                    var addresses = fields[1].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var additionalLocations = new List<IncomingAdditionalLocation>(addresses.Length);
                    additionalLocations.AddRange(addresses.Select(address => new IncomingAdditionalLocation { Email = address }));
                    incomingOrderModel.AdditionalLocation = additionalLocations;

                    incomingOrderModel.idWebinar = int.Parse(fields[2]);
                    incomingOrderModel.idRegType = int.Parse(fields[3]);
                    incomingOrderModel.idAffiliate = int.Parse(fields[4]);
                    incomingOrderModel.FirstName = fields[5];
                    incomingOrderModel.LastName = fields[6];
                    incomingOrderModel.Institution = fields[7];
                    incomingOrderModel.Title = fields[8]; ;
                    incomingOrderModel.Email = fields[9];
                    incomingOrderModel.BillingAddress.Name = string.Concat(incomingOrderModel.FirstName, " ", incomingOrderModel.LastName);
                    incomingOrderModel.BillingAddress.Phone = fields[10];
                    incomingOrderModel.BillingAddress.StreetAddress = fields[11];
                    incomingOrderModel.BillingAddress.StreetAddress2 = fields[12];
                    incomingOrderModel.BillingAddress.City = fields[13];
                    incomingOrderModel.BillingAddress.State = fields[14];
                    incomingOrderModel.BillingAddress.Zip = fields[15];
                    incomingOrderModel.BillingAddress.Country = fields[16];
                    incomingOrderModel.BillingAddress.AddressType = WebUiConstants.BillingAddress;

                    //  If there is a name for the shipping address, we need to parse the address
                    var name = fields[17];

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        CopyBillingAddressToShippingAddress(ref incomingOrderModel);
                        incomingOrderModels.Add(incomingOrderModel);
                    }
                    else
                    {
                        incomingOrderModel.ShippingAddress.Name = name;
                        incomingOrderModel.ShippingAddress.Phone = fields[18];
                        incomingOrderModel.ShippingAddress.StreetAddress = fields[19];
                        incomingOrderModel.ShippingAddress.StreetAddress2 = fields[20];
                        incomingOrderModel.ShippingAddress.City = fields[21];
                        incomingOrderModel.ShippingAddress.State = fields[22];
                        incomingOrderModel.ShippingAddress.Zip = fields[23];
                        incomingOrderModel.ShippingAddress.Country = fields[24];
                        incomingOrderModel.ShippingAddress.AddressType = WebUiConstants.ShippingAddress;
                    }                    
                });

                return incomingOrderModels;
            }

        }

        private static void CopyBillingAddressToShippingAddress(ref IncomingOrderModel incomingOrderModel)
        {
            incomingOrderModel.ShippingAddress.AddressType = WebUiConstants.ShippingAddress;
            incomingOrderModel.ShippingAddress.City = incomingOrderModel.BillingAddress.City;
            incomingOrderModel.ShippingAddress.Country = incomingOrderModel.BillingAddress.Country;
            incomingOrderModel.ShippingAddress.Name = incomingOrderModel.BillingAddress.Name;
            incomingOrderModel.ShippingAddress.Phone = incomingOrderModel.BillingAddress.Phone;
            incomingOrderModel.ShippingAddress.State = incomingOrderModel.BillingAddress.State;
            incomingOrderModel.ShippingAddress.StreetAddress = incomingOrderModel.BillingAddress.StreetAddress;
            incomingOrderModel.ShippingAddress.StreetAddress2 = incomingOrderModel.BillingAddress.StreetAddress2;
            incomingOrderModel.ShippingAddress.Zip = incomingOrderModel.BillingAddress.Zip;
        }
    }
}