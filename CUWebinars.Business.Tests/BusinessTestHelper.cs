using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CUWebinars.Business.Models;
using Ninject;

namespace CUWebinars.Business.Tests
{
    public class BusinessTestHelper
    {
        private static readonly Assembly _serviceAssembly = Assembly.Load("CUWebinars.Business.Tests");
        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        public static void AutoRegisterType(Type type, IKernel kernel)
        {
            var handlerRegistrations = _serviceAssembly.GetExportedTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.ContainsGenericParameters)
                .SelectMany(x => x.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == type)
                    .Select(i => new { service = i, implementation = x })
                );

            foreach (var registration in handlerRegistrations)
            {
                var abstraction = registration.service; //  interface
                var implementation = registration.implementation; // class inplementation
                kernel.Bind(abstraction).To(implementation).InTransientScope();
            }
        }

        public static IList<Address> GetAddresses(string fullName)
        {
            var billingAddress = new Address
            {
                AddressType = "Billing",
                Name = fullName,
                City = "Dallas",
                Zip = "75201",
                State = "TX",
                StreetAddress = "1 Liberty St",
                Country = "USA"
            };

            var shippingAddress = new Address
            {
                AddressType = "Shipping",
                Name = fullName,
                City = "Dallas",
                Zip = "75201",
                State = "TX",
                StreetAddress = "1 Liberty St",
                Country = "USA"
            };

            return new[] { billingAddress, shippingAddress };
        }

        /// <summary>
        /// Taken from StackOverflow answer http://stackoverflow.com/a/1122519/540156
        /// </summary>
        public static string GetRandomString(int size)
        {
            var builder = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static void PopulateOrder(Order newOrder, WebUser webUser)
        {
            newOrder.AdminComments = "incomingOrderModel.AdminComments";
            newOrder.AffiliateComments = "AffiliateComments";
            newOrder.UserComments = "incomingOrderModel.UserComments";
            newOrder.Origin = "incomingOrderModel.Origin";
            newOrder.FirstName = webUser.FirstName;
            newOrder.LastName = webUser.LastName;
            newOrder.Institution = webUser.Institution.InstitutionName;
            newOrder.BillingEmail = webUser.email;

            newOrder.BillingAddress = "968 Wildcat Dr";
            newOrder.BillingAddress2 = null;
            newOrder.BillingPhone = "555-555-5555";
            newOrder.BillingCity = "Del Rio";
            newOrder.BillingState = "Tx";
            newOrder.BillingZip = "5000";

            newOrder.ShippingAddress = "968 Wildcat Dr";
            newOrder.ShippingAddress2 = null;
            newOrder.ShippingPhone = "555-555-5555";
            newOrder.ShippingCity = "Del Rio";
            newOrder.ShippingState = "Tx";
            newOrder.ShippingZip = "5000";
            newOrder.ShippingFirstName = "Alan";
            newOrder.ShippingLastName = "Turing";
        }
    }
}