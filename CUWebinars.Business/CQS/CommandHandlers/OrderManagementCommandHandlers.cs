using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
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
        ICommandHandler<RegisterNewAccountCommand>,
        ICommandHandler<VerifyAccountCommand>,
        ICommandHandler<AddOrderCommand>
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
                var price = GetPriceOfAdditionalLocation(command.Webinar.idWebinar);

                //string email,decimal price,string fullname

                foreach (var additionalLocation in command.AdditionalLocations)
                {
                    var additionalLocationEmail = additionalLocation.Email;

                    additionalLocations.Add(_orderManagementService.CreateAdditionalLocation(
                        additionalLocationEmail, 
                        price,
                        null)//field for FullName
                        );
                    //additionalLocationFirstName + ' ' + additionalLocationLastName)
                }
            }

             var orderRow = _orderManagementService.CreateOrderRow(command.Webinar, 
                additionalLocations,
                command.RegistrationType
                );

            orderRow.Discount = _orderManagementService.GetDiscount(command.Email);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderRow = orderRow;
            };

            _postCommitRegistrator.ExecuteActions();
            _postCommitRegistrator.Reset();
        }

        private decimal GetPriceOfAdditionalLocation(int idWebinar)
        {
            //Todo: hit lookup table that contained addLoc price per the given webinar
            return 150;
        }


        public void Handle(RegisterNewAccountCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var institutionForUser = _membershipService.ProcessInstitutionForUser(command.Institution
                    , command.Email
                    ,command.BillingAddress.City
                    ,command.BillingAddress.State
                    ,"N"
                    ,"New"
                    ,command.BillingAddress.Zip
                    );

            USTimeZone userTimeZone = _membershipService.GetTimeZoneByZip();

            command.BillingAddress.AddressType = DomainConstants.BillingAddress;
            command.ShippingAddress.AddressType = DomainConstants.ShippingAddress;

            var addresses = new List<Address> {command.BillingAddress, command.ShippingAddress};

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
            var importedOrder = _orderManagementService.CreateNewOrder(command.Affiliate,command.WebUser, command.Webinar, command.OrderRow);

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

            _orderManagementService.SaveOrderChanges(importedOrder, command.VerificationKey, command.ConfirmChangeEmailUrl);

            _postCommitRegistrator.Committed += () =>
            {
                command.OrderId = importedOrder.idOrder;
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
