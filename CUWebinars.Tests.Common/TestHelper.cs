using System.Text;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Services;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System;
using System.Linq;
using System.Web;

namespace CUWebinars.Tests.Common
{
    public class TestHelper
    {
        public static IMembershipService CreateMembershipService()
        {
            ILogger logger = new Log4NetLogger(typeof (MembershipService));
            var ctx = new TTSWebinarsContext();
            var refDataRepository = new RefDataRepository();
            var config = MembershipRebootConfig.Create(
                HttpRuntime.AppDomainAppPath, new StateService()
                );
            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());

            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx),
                new RefDataRepository(),
                new SamAuthenticationService(userAccountService),
                userAccountService,
                new WebUserRepository(ctx),
                logger
                );

            return membershipService;
        }

        public static IOrderManagementService CreateOrderManagemetService()
        {
            var ctx = new TTSWebinarsContext();
            var ttsConfig = new TtsConfiguration();

            IOrderManagementService orderManagementService = new OrderManagementService(new AffiliateRepository(ctx),
                new RegTypeRepository(ctx),
                new OrderRepository(ctx),
                new RefDataRepository(),
                new WebUserRepository(ctx),
                new WebinarRepository(ctx),
                new AdditionalLocationRepository(ctx),
                new Log4NetLogger(typeof (OrderManagementService)),
                ttsConfig
                );

            return orderManagementService;
        }

        public static string ExtractEmailAddress(string queryString)
        {
            var indexOfEmail = queryString.IndexOf("Email=", StringComparison.OrdinalIgnoreCase);

            var email = new string(queryString.Skip(indexOfEmail + 6).TakeWhile(c => c != '&').ToArray());

            return email;
        }

        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);

        /// <summary>
        /// Taken from StackOverflow answer http://stackoverflow.com/a/1122519/540156
        /// </summary>
        public static string RandomString(int size)
        {
            var builder = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*Random.NextDouble() + 97)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        public static JObject TransformQueryStringToJsonCompliantString(string queryString)
        {
            var queryStringValues = queryString.Split(Convert.ToChar("&"));

            string json = "{ ";

            foreach (var queryStringValue in queryStringValues)
            {
                var key = new String(queryStringValue.TakeWhile(c => c != '=').ToArray());
                var value = new String(queryStringValue.SkipWhile(c => c != '=').Skip(1).ToArray());

                if (key.StartsWith("id"))
                {
                    json += "'" + key + "':" + value + ",";
                }
                else
                {
                    json += "'" + key + "': '" + value + "',";
                }
            }

            json = json.Remove(json.Length - 1);
            json += "}";

            var jObject = JObject.Parse(@json);

            return jObject;
        }
    }
}