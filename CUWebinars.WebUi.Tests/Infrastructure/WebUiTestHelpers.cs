//using System;
//using System.IO;
//using System.Text;
//using BrockAllen.MembershipReboot;
//using BrockAllen.MembershipReboot.Ef;
//using BrockAllen.MembershipReboot.WebHost;
//using CUWebinars.Business.AccountService;
//using CUWebinars.Business.Models;
//using CUWebinars.Business.Repository;

//namespace CUWebinars.WebUi.Tests
//{
//    internal class WebUiTestHelpers
//    {
//        private static IMembershipService CreateMembershipService()
//        {
//            var ctx = new TTSWebinarsContext();
//            var memRebootCtx = new DefaultMembershipRebootDatabase();
//            var refDataRepository = new RefDataRepository();
//            var config =
//                MembershipRebootConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"\..\.."),
//                    new StateService(), refDataRepository);
//            var userAccountService = new UserAccountService(config, new DefaultUserAccountRepository());

//            IMembershipService membershipService = new MembershipService(new InstitutionRepository(ctx),
//                new RefDataRepository(),
//                new SamAuthenticationService(userAccountService),
//                userAccountService,
//                new WebUserRepository(ctx)
//                );
//            return membershipService;
//        }

//        private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

//        /// <summary>
//        /// Taken from StackOverflow answer http://stackoverflow.com/a/1122519/540156
//        /// </summary>
//        internal static string RandomString(int size)
//        {
//            var builder = new StringBuilder(size);
//            for (var i = 0; i < size; i++)
//            {
//                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
//                builder.Append(ch);
//            }

//            return builder.ToString();
//        }
//    }
//}
