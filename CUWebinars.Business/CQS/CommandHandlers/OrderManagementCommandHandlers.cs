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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                var priceOfAdditionalLocationListItem = command.Webinar.AdditionalLocationPrice;

                decimal priceOfAdditionalLocation = 0M;

                priceOfAdditionalLocation = priceOfAdditionalLocationListItem;

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

            if (!string.IsNullOrWhiteSpace(command.AdditionalLocationsString) && command.AdditionalLocationsString != "NULL")
            {

                string[] addLocs = command.AdditionalLocationsString.Split(',');

                var addLocPrice = _orderManagementService.GetAdditionalLocationsPricing(command.Webinar.idWebinar);

                foreach (var additionalLocationEmail in addLocs)
                {
                    if (string.IsNullOrEmpty(additionalLocationEmail) != true)
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

            if (orderRow != null)
            {
                if (command.Discount != null)
                {
                    orderRow.Discount = _orderManagementService.GetDiscountById(Convert.ToInt32(command.Discount.Replace("CP_", "")));

                }

                _postCommitRegistrator.Committed += () =>
                {
                    command.OrderRow = orderRow;
                };

            }

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }

        public void Handle(ImportOrderRowCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            IList<AdditionalLocation> additionalLocations = new List<AdditionalLocation>();

            if (!string.IsNullOrWhiteSpace(command.AdditionalLocationsString) && command.AdditionalLocationsString != "NULL")
            {

                string[] addLocs = command.AdditionalLocationsString.Split(',');
                foreach (var additionalLocationEmail in addLocs)
                {
                    additionalLocations.Add(_orderManagementService.CreateAdditionalLocation(
                        additionalLocationEmail,
                        0,
                        null) // field for FullName
                        );
                }
            }

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

            switch (command.CreationMode)
            {
                case DomainConstants.CreationModeImported:
                    //_membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.CartRegistration);
                    _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.OrderImportRegistration);
                    break;
                case DomainConstants.CreationModeMigrated:
                    _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.OrderMigrated);
                    break;
                default:
                    break;
            }

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
            var importedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser,
                command.Webinar, command.OrderRow);

            if (!ReferenceEquals(null, importedOrder))
            {


                JObject existingJObject = null;

                string comments = string.Empty;


                if (!ReferenceEquals(null, importedOrder.AdminComments))
                {
                    comments = importedOrder.AdminComments.Trim();
                }

                var newJson =
                    new JProperty(
                        string.Concat("LegacyCommentsAddOrder-", DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                        new JObject(

                            new JProperty("LegacyComments", importedOrder.AdminComments)
                            ));

                if (string.IsNullOrWhiteSpace(comments))
                {
                    existingJObject = new JObject(newJson);
                }
                else
                {
                    existingJObject = JObject.Parse(comments);
                    existingJObject.Add(newJson);
                }

                importedOrder.AdminComments = existingJObject.ToString(Formatting.None);


                importedOrder.AdminComments = command.AdminComments;
                importedOrder.AffiliateComments = command.AffiliateComments;
                importedOrder.UserComments = command.UserComments;
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


                _orderManagementService.GetJoinUrl(command.OrderRow); ;

                _membershipService.AddClaimForPostEventMaterials(importedOrder.BillingEmail, command.OrderRow,
                _orderManagementService.CalculatePostEventMaterialsAccessExpiry(command.OrderRow), command.Tenant);


                _orderManagementService.SaveOrderChanges(importedOrder, command.VerificationKey,
                    command.ConfirmChangeEmailUrl, command.OrderGenesis);

                _postCommitRegistrator.Committed += () =>
                {
                    command.OrderId = importedOrder.idOrder;
                };

                _postCommitRegistrator.ExecuteActions();
                _postCommitRegistrator.Reset();
            }
        }

        public void Handle(ImportOrderCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            //prevents re-importation
            //var userAlreadyHasOrder = _orderManagementService.GetOrdersByUserId(command.WebUser.idUser).Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == command.Webinar.idWebinar) != null).SingleOrDefault();
            var importedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser,
                command.Webinar, command.OrderRow, DomainConstants.OriginImported);
            ;
            //if (ReferenceEquals(null, userAlreadyHasOrder))
            //{

            if (command.OrderGenesis == OrderGenesis.ImportedForACSExistingUser
                || command.OrderGenesis == OrderGenesis.ImportedForACSNewUser)
            {
                importedOrder.Origin = DomainConstants.OriginImportedACS;
            }

            string buildMessage = "Imported: " + DateTime.UtcNow;

            if (!ReferenceEquals(null, importedOrder))
            {
                //following copies pattern found at WebinarController | Identify
                JObject existingJObject = null;
                JObject userJObject = null;

                string comments = string.Empty;
                string uComments = string.Empty;


                if (!ReferenceEquals(null, importedOrder.AdminComments))
                {
                    comments = importedOrder.AdminComments.Trim();
                }

                var newJson =
                    new JProperty(
                        string.Concat("Imported-", DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                        new JObject(
                            new JProperty("ImportedOrder", command.Affiliate.ttsDomain),
                            new JProperty("Details", buildMessage)
                            ));
                userJObject = new JObject(newJson);

                if (string.IsNullOrWhiteSpace(comments))
                {
                    existingJObject = new JObject(newJson);
                }
                else
                {
                    existingJObject = JObject.Parse(comments);
                    existingJObject.Add(newJson);
                }

                importedOrder.AdminComments = existingJObject.ToString(Formatting.None);
                importedOrder.UserComments = userJObject.ToString(Formatting.None);
                importedOrder.AffiliateComments = userJObject.ToString(Formatting.None);
            }
            importedOrder.OrderDate = command.OrderDate;
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

            _membershipService.AddClaimForPostEventMaterials(importedOrder.BillingEmail, command.OrderRow,
            _orderManagementService.CalculatePostEventMaterialsAccessExpiry(command.OrderRow), command.Tenant);


            _orderManagementService.SaveOrderChanges(importedOrder
                , command.VerificationKey
                , command.ConfirmChangeEmailUrl);

            //_orderManagementService.SaveOrderChanges(
            //    importedOrder,
            //    command.VerificationKey,
            //    //why are we using confirmChangeEmailURL instead of AddPasswordURL?
            //    command.ConfirmChangeEmailUrl,
            //    command.OrderGenesis
            //    );

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderId = importedOrder.idOrder;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
            //}
        }
        public void Handle(MigrateOrderCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            try
            {
                var migratedOrder = _orderManagementService.CreateNewOrder(command.Affiliate, command.WebUser,
                    command.Webinar, command.OrderRow, DomainConstants.OriginMigrated);


                migratedOrder.Total = command.Total;
                
                string buildMessage = "MigratedOn: " + DateTime.UtcNow + string.Format(" OrginalTotal: {0}", command.Total);


                //following copies pattern found at WebinarController | Identify
                JObject existingJObject = null;
                JObject userJObject = null;

                string comments = string.Empty;
                string uComments = string.Empty;


                if (!ReferenceEquals(null, migratedOrder.AdminComments))
                {
                    comments = migratedOrder.AdminComments.Trim();
                }

                var newJson =
                    new JProperty(
                        string.Concat("Migrated-", DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                        new JObject(
                            new JProperty("MigratedOrder", command.Affiliate.ttsDomain),
                            new JProperty("Details", buildMessage)
                        ));
                userJObject = new JObject(newJson);

                if (string.IsNullOrWhiteSpace(comments))
                {
                    existingJObject = new JObject(newJson);
                }
                else
                {
                    existingJObject = JObject.Parse(comments);
                    existingJObject.Add(newJson);
                }

                migratedOrder.AdminComments = existingJObject.ToString(Formatting.None);
                migratedOrder.UserComments = userJObject.ToString(Formatting.None);
                migratedOrder.AffiliateComments = userJObject.ToString(Formatting.None);

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


                _membershipService.AddClaimForPostEventMaterials(migratedOrder.BillingEmail, command.OrderRow,
                    _orderManagementService.CalculatePostEventMaterialsAccessExpiry(command.OrderRow), command.Tenant);


                //_orderManagementService.SaveOrderChanges(migratedOrder, command.VerificationKey, command.ConfirmChangeEmailUrl);
                _orderManagementService.SaveChanges();

                _postCommitRegistrator.Committed += () =>
                {
                    command.OrderId = migratedOrder.idOrder;
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

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
