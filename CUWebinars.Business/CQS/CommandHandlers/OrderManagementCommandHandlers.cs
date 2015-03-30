using System.Configuration;
using System.Diagnostics;
using System.Runtime.Caching.Configuration;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.CQS.Commands;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using DDay.iCal;

namespace CUWebinars.Business.CQS.CommandHandlers
{
    public class OrderManagementCommandHandlers :
        ICommandHandler<AddOrderRowCommand>,
        ICommandHandler<MigrateOrderRowCommand>,
        ICommandHandler<ImportOrderRowCommand>,
        ICommandHandler<RegisterNewAccountCommand>,
        ICommandHandler<VerifyAccountCommand>,
        ICommandHandler<AddOrderCommand>,
        ICommandHandler<MigrateOrderCommand>,
        ICommandHandler<ImportOrderCommand>
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IMembershipService _membershipService;
        private readonly PostCommitRegistrator _postCommitRegistrator;
        private bool _disposed;

        public OrderManagementCommandHandlers(IOrderManagementService orderManagementService,
            IMembershipService membershipService,
            PostCommitRegistrator postCommitRegistrator)
        {
            _orderManagementService = orderManagementService;
            _membershipService = membershipService;
            _postCommitRegistrator = postCommitRegistrator;
        }

        public void Handle(AddOrderRowCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            IList<AdditionalLocation> additionalLocations = new List<AdditionalLocation>();

            if (command.AdditionalLocations != null && command.AdditionalLocations.Any())
            {
                var priceOfAdditionalLocationListItem = GetPriceOfAdditionalLocation(command.Webinar.idWebinar).SingleOrDefault();

                decimal priceOfAdditionalLocation = 0M;

                if (!ReferenceEquals(priceOfAdditionalLocationListItem, null))
                {
                    priceOfAdditionalLocation = priceOfAdditionalLocationListItem.Price; 
                }

                foreach (var additionalLocationEmail in command.AdditionalLocations.Select(additionalLocation => additionalLocation.Email))
                {
                    additionalLocations.Add(_orderManagementService.CreateAdditionalLocation(
                        additionalLocationEmail,
                        priceOfAdditionalLocation,
                        null) // field for FullName
                        );
                }
            }

            var orderRow = _orderManagementService.CreateOrderRow(command.Webinar,
               additionalLocations,
               command.RegistrationType
               );

            //orderRow.Discount = _orderManagementService.GetDiscount(command.Email);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderRow = orderRow;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }

        public void Handle(MigrateOrderRowCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            IList<AdditionalLocation> additionalLocations = new List<AdditionalLocation>();
            //CUWEBINARS VS BANKWEBINARS

            //         for idRegType mapping from legacy to new
            //200	Live Plus Five	Live_Session_Only_1Hr_165_97
            //201	OnDemand Recording Only	OnDemand_Recording_Only_1Hr_185_201
            //202	CD-ROM and Hardcopy Handouts	Live_Plus_OnDemand_Weblinks_1Hr_235_202
            //203	Live Plus Six	CD-ROM_and_Hardcopy_Handouts_1Hr_215_203
            //204	Premier Package	Premier_Package_1Hr_265_204
            //205	Live Plus Five	Live_Session_Only_2Hr_265_205
            //206	OnDemand Recording Only	On-Demand_Recording_Only_2Hr_295_206
            //207	Live Plus Six	Live_Plus_OnDemand_Weblinks_2Hr_365_207
            //208	CD-ROM and Hardcopy Handouts	CD-ROM_and_Hardcopy_Handouts_2Hr_325_208
            //209	Premier Package	Premier_Package_2Hr_395_209

            if (command.OrderDate > Convert.ToDateTime("01-01-2015") && command.RegistrationType < 200)
            {
                try
                {
                    int idRegType = Convert.ToInt32(command.RegistrationType);
                    switch (idRegType)
                    {
                        case 1: { idRegType = 205; break; }
                            ;
                        case 16: { idRegType = 206; break; }
                            ;
                        case 3: { idRegType = 207; break; }
                            ;
                        case 17: { idRegType = 208; break; }
                            ;
                        case 18: { idRegType = 209; break; }
                            ;
                        ////1hr
                        case 27: { idRegType = 200; break; }
                            ;
                        case 32: { idRegType = 201; break; }
                            ;
                        case 35: { idRegType = 202; break; }
                            ;
                        case 33: { idRegType = 203; break; }
                            ;
                        case 36: { idRegType = 204; break; }
                        //    ;
                        //default: { idRegType = 0; break; }
                    }

                    command.RegistrationType = idRegType;
                }
                catch (Exception)
                {

                    throw;
                }

            }

            if (!string.IsNullOrWhiteSpace(command.AdditionalLocationsString) && command.AdditionalLocationsString != "NULL")
            {

                string[] addLocs = command.AdditionalLocationsString.Split(',');

                var addLocPrice = _orderManagementService.GetPriceOfAdditionalLocation(command.Webinar.idWebinar);

                foreach (var additionalLocationEmail in addLocs)
                {
                    additionalLocations.Add(_orderManagementService.CreateAdditionalLocation(
                        additionalLocationEmail,
                        addLocPrice,
                        command.Name) // field for FullName
                        );
                }
            }

            var orderRow = _orderManagementService.CreateOrderRow(command.Webinar,
               additionalLocations,
               command.RegistrationType
               );

            orderRow.Discount = _orderManagementService.GetDiscountById(Convert.ToInt32(command.Discount));

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderRow = orderRow;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }

        public void Handle(ImportOrderRowCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            IList<AdditionalLocation> additionalLocations = new List<AdditionalLocation>();

            //if (!string.IsNullOrWhiteSpace(command.AdditionalLocationsString) && command.AdditionalLocationsString != "NULL")
            //{

            //    string[] addLocs = command.AdditionalLocationsString.Split(',');
            //    foreach (var additionalLocationEmail in addLocs)
            //    {
            //        additionalLocations.Add(_orderManagementService.CreateAdditionalLocation(
            //            additionalLocationEmail,
            //            0,
            //            null) // field for FullName
            //            );
            //    }
            //}

            var orderRow = _orderManagementService.CreateOrderRow(command.Webinar,
               additionalLocations,
               command.RegistrationType
               );

            orderRow.Discount = _orderManagementService.GetDiscountByCode(command.Discount);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderRow = orderRow;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }

        private IList<AdditionalLocationsPricing> GetPriceOfAdditionalLocation(int idWebinar)
        {
            DataOperations dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(idWebinar);

            return additionalLocationsPricing;
        }


        public void Handle(RegisterNewAccountCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var institutionForUser = _membershipService.ProcessInstitutionForUser(command.Institution
                    , command.Email
                    , command.BillingAddress.City
                    , command.BillingAddress.State
                    , "N"
                    , "New"
                    , command.BillingAddress.Zip
                    );

            USTimeZone userTimeZone = _membershipService.GetTimeZoneByZip();

            command.BillingAddress.AddressType = DomainConstants.BillingAddress;
            command.ShippingAddress.AddressType = DomainConstants.ShippingAddress;

            var addresses = new List<Address> { command.BillingAddress, command.ShippingAddress };

            var webUser = _membershipService.CreateWebUser(command.Tenant
                , command.FirstName
                , command.LastName
                , command.TempPassword
                , command.Email
                , userTimeZone
                , UserType.Customer
                , institutionForUser.idInstitution
                , addresses
                , command.Title ?? null
                , null
                , DomainConstants.Active
                );

            var userAccount = _membershipService.CreateUser(command.Tenant
                , command.FirstName
                , command.LastName
                , command.Email
                , command.TempPassword
                , command.Email
                );

            _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.OrderImportRegistration);

            _postCommitRegistrator.Committed += () =>
            {
                command.WebUser = webUser;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();


        }

        public void Handle(VerifyAccountCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var userAccount = _membershipService.VerifyEmailFromKey(
                            command.VerificationKey,
                            command.TempPassword
                            );
        }

        public void Handle(AddOrderCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var importedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser, command.Webinar, command.OrderRow);

            importedOrder.AdminComments = "incomingOrderModel.AdminComments";
            importedOrder.AffiliateComments = command.AffiliateComments;
            importedOrder.UserComments = "incomingOrderModel.UserComments";
            importedOrder.Origin = "incomingOrderModel.Origin";
            importedOrder.FirstName = command.FirstName;
            importedOrder.LastName = command.LastName;
            importedOrder.Institution = command.WebUser.Institution.InstitutionName;
            importedOrder.BillingEmail = command.Email;

            importedOrder.BillingAddress = command.BillingAddress.StreetAddress;
            importedOrder.BillingAddress2 = command.BillingAddress.StreetAddress2;
            importedOrder.BillingPhone = command.BillingAddress.Phone;
            importedOrder.BillingCity = command.BillingAddress.City;
            importedOrder.BillingState = command.BillingAddress.State;
            importedOrder.BillingZip = command.BillingAddress.Zip;

            importedOrder.ShippingAddress = command.ShippingAddress.StreetAddress;
            importedOrder.ShippingAddress2 = command.ShippingAddress.StreetAddress2;
            importedOrder.ShippingPhone = command.ShippingAddress.Phone;
            importedOrder.ShippingCity = command.ShippingAddress.City;
            importedOrder.ShippingState = command.ShippingAddress.State;
            importedOrder.ShippingZip = command.ShippingAddress.Zip;
            importedOrder.ShippingFirstName = command.FirstName;
            importedOrder.ShippingLastName = command.LastName;


            _orderManagementService.GetJoinUrl(command.OrderRow);


            _orderManagementService.SaveOrderChanges(importedOrder, command.VerificationKey, command.ConfirmChangeEmailUrl, command.OrderGenesis);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderId = importedOrder.idOrder;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }
        public void Handle(ImportOrderCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            //prevents re-importation
            var userAlreadyHasOrder = _orderManagementService.GetOrdersByUserId(command.WebUser.idUser)
                .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == command.Webinar.idWebinar) != null).SingleOrDefault(); 
            ;
            if (ReferenceEquals(null, userAlreadyHasOrder))
            {

                var importedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser,
                    command.Webinar, command.OrderRow, DomainConstants.OriginImported);

                importedOrder.OrderDate = command.OrderDate;
                importedOrder.AdminComments = string.Format("Imported On: {0}\r\n", DateTime.Now.ToShortDateString());
                importedOrder.AffiliateComments = command.AffiliateComments;
                importedOrder.UserComments = "";
                importedOrder.OrderStatus = OrderStatus.Submitted;
                importedOrder.FirstName = command.FirstName;
                importedOrder.LastName = command.LastName;
                importedOrder.Institution = command.WebUser.Institution.InstitutionName;
                importedOrder.BillingEmail = command.Email;

                importedOrder.BillingAddress = command.BillingAddress.StreetAddress;
                importedOrder.BillingAddress2 = command.BillingAddress.StreetAddress2;
                importedOrder.BillingPhone = command.BillingAddress.Phone;
                importedOrder.BillingCity = command.BillingAddress.City;
                importedOrder.BillingState = command.BillingAddress.State;
                importedOrder.BillingZip = command.BillingAddress.Zip;

                importedOrder.ShippingAddress = command.ShippingAddress.StreetAddress;
                importedOrder.ShippingAddress2 = command.ShippingAddress.StreetAddress2;
                importedOrder.ShippingPhone = command.ShippingAddress.Phone;
                importedOrder.ShippingCity = command.ShippingAddress.City;
                importedOrder.ShippingState = command.ShippingAddress.State;
                importedOrder.ShippingZip = command.ShippingAddress.Zip;
                importedOrder.ShippingFirstName = command.FirstName;
                importedOrder.ShippingLastName = command.LastName;

                _orderManagementService.GetJoinUrl(command.OrderRow);

                _orderManagementService.SaveOrderChanges(
                    importedOrder,
                    command.VerificationKey,
                    command.ConfirmChangeEmailUrl,
                    command.OrderGenesis
                    );

                _postCommitRegistrator.Committed += () =>
                {
                    command.OrderId = importedOrder.idOrder;
                };

                _postCommitRegistrator.ExecuteActions();
                _postCommitRegistrator.Reset();
            }
        }
        public void Handle(MigrateOrderCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var migratedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser,
                command.Webinar, command.OrderRow, DomainConstants.OriginMigrated);

            migratedOrder.Total = command.Total;

            migratedOrder.AdminComments = string.Format("MigratedOn: {0}\r\n", DateTime.Now.ToShortDateString());
            migratedOrder.AdminComments += string.Format("OrginalTotal: {0}\r\n", command.Total);
            //migratedOrder.AdminComments += string.Format("OrginalUserID: {0}\r\n", command.idUserLegacy);
            migratedOrder.AffiliateComments = command.AffiliateComments;
            migratedOrder.UserComments = "";

            migratedOrder.OrderStatus = OrderStatus.Submitted;
            migratedOrder.idOrderLegacy = command.idOrderLegacy;
            migratedOrder.OrderDate = command.OrderDate;
            migratedOrder.FirstName = command.FirstName;
            migratedOrder.LastName = command.LastName;
            migratedOrder.Institution = command.WebUser.Institution.InstitutionName;
            migratedOrder.BillingEmail = command.Email;

            migratedOrder.BillingAddress = command.BillingAddress.StreetAddress;
            migratedOrder.BillingAddress2 = command.BillingAddress.StreetAddress2;
            migratedOrder.BillingPhone = command.BillingAddress.Phone;
            migratedOrder.BillingCity = command.BillingAddress.City;
            migratedOrder.BillingState = command.BillingAddress.State;
            migratedOrder.BillingZip = command.BillingAddress.Zip;

            migratedOrder.ShippingAddress = command.ShippingAddress.StreetAddress;
            migratedOrder.ShippingAddress2 = command.ShippingAddress.StreetAddress2;
            migratedOrder.ShippingPhone = command.ShippingAddress.Phone;
            migratedOrder.ShippingCity = command.ShippingAddress.City;
            migratedOrder.ShippingState = command.ShippingAddress.State;
            migratedOrder.ShippingZip = command.ShippingAddress.Zip;
            migratedOrder.ShippingFirstName = command.FirstName;
            migratedOrder.ShippingLastName = command.LastName;

            _orderManagementService.GetJoinUrl(command.OrderRow);

            _orderManagementService.SaveOrderChanges(migratedOrder, command.VerificationKey, command.ConfirmChangeEmailUrl);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderId = migratedOrder.idOrder;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _membershipService.Dispose();
                _orderManagementService.Dispose();

            }
            _disposed = true;
        }
    }
}
