using System;
using CUWebinars.Business.Models;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Text;
using CUWebinars.Business.Core;

namespace CUWebinars.Business.Repository
{
    public class WebUserRepository : TTSWebinarsRepository<TTSWebinarsContext, WebUser>, IWebUserRepository
    {
        private readonly ILogger _logger;

        public WebUserRepository(TTSWebinarsContext ttsWebinarsContext)
        {

        }

        public WebUserRepository(TTSWebinarsContext ctx, ILogger logger)
            : base(ctx)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds the Highest Id currently in use so newly created WebUser objects have an Id.
        /// Id value is not db-generated so that objects created by the seed method 
        /// can more easily import the Id used by the old system - let's us use the same Affiliate and Presenter
        /// Ids that we've grown used to using.
        /// </summary>
        /// <returns></returns>
        public int FindHighestUserId()
        {
            int nextId = items
                .OrderByDescending(i => i.idUser)
                .Take(1)
                .Select(i => i.idUser).Single();

            return nextId++;
        }

        public int? FindInstitution(string zip, string institutionName)
        {
            int myInst = ((TTSWebinarsContext)db).Institutions
                .Where(i => i.InstitutionName == institutionName && i.Zip == zip)
                .Select(i => i.idInstitution)
                .SingleOrDefault();

            return myInst;
        }

        public WebUser FindByIdLoaded(int id)
        {
            var webUser = items
                .Include(i => i.Addresses)
                //.Include(i => i.Affiliate)
                .Include(i => i.Institution)
                .Include(i => i.Presenter)
                .Where(i => i.idUser == id);
            return webUser.FirstOrDefault();
        }

        public WebUser GetWebUserByEmail(string email)
        {
            return items
                .Include(wu => wu.Addresses)
                .Include(wu => wu.Institution)
                .Where(w => w.email == email).SingleOrDefault();
        }



        public WebUser GetWebUserByEmailDomain(string emailDomain)
        {
            return items
                .Include(wu => wu.Addresses)
                .Include(wu => wu.Institution)
                .FirstOrDefault(w => w.email.EndsWith(emailDomain));
        }

        public IList<WebUser> GetWebUsersOfDiscount(int idDiscount)
        {
            var users = ((TTSWebinarsContext)db).WebUsers
                .Where(u => u.idSubscriptionDiscount == idDiscount).ToList();

            return users;

        }

        public WebUser SetWebUserWSP(int idDiscount, int idUser)
        {
            var user = ((TTSWebinarsContext)db).WebUsers
                .Where(u => u.idUser == idUser)
                .FirstOrDefault();

            user.idSubscriptionDiscount = idDiscount;
            return user;

        }

        public Affiliate FindAffiliateForSession(string identity)
        {
            try
            {
                IList<int> affiliateIds = null;
                int idUser = ((TTSWebinarsContext)db).Orders
                    .Where(o => o.BillingEmail == identity)
                    .Select(o => o.idUser).FirstOrDefault();

                if (idUser < 0)
                {
                    return null;
                }

                affiliateIds = ((TTSWebinarsContext)db).Orders
                    .Where(o => o.BillingEmail == identity)
                    .OrderByDescending(o => o.OrderDate)
                    .Include(u => u.WebUser)
                    .Select(o => o.idAffiliate)
                    .Distinct()
                    .ToList();

                if (affiliateIds.Any())
                {
                    //  get the most recent
                    int affiliateIdForOrder, mostRecentAffiliateId;
                    affiliateIdForOrder = mostRecentAffiliateId = affiliateIds.First();

                    // The history is of more than 1 affiliate
                    if (affiliateIds.Count() > 1)
                    {
                        // The business rule is that where there is more than one Affiliate which the 
                        // user has made orders for, if one affiliate has been used twice as many times 
                        // as the most recent Affiliate, then make the order for that Affiliate.

                        var groups = affiliateIds.GroupBy(a => a);
                        //
                        int mostUsedAffiliateId = 0;
                        int mostUses = 0;
                        int numberOfUsesOfMostRecentAffiliate = 0;
                        int current = 0;
                        foreach (var group in groups)
                        {
                            var ordersByAffForThisUser = ((TTSWebinarsContext)db).Orders
                                .Where(o => o.BillingEmail == identity
                                            && o.idAffiliate == group.Key
                                            && (o.OrderStatus == OrderStatus.Paid
                                            || o.OrderStatus == OrderStatus.Billed
                                            || o.OrderStatus == OrderStatus.Submitted
                                            ))
                                .ToList().Count;

                            numberOfUsesOfMostRecentAffiliate = ((TTSWebinarsContext)db).Orders
                                .Where(o => o.BillingEmail == identity
                                            && o.idAffiliate == affiliateIds.FirstOrDefault()
                                            && (o.OrderStatus == OrderStatus.Paid
                                            || o.OrderStatus == OrderStatus.Billed
                                            || o.OrderStatus == OrderStatus.Submitted
                                            ))
                                .ToList().Count;

                            if (ordersByAffForThisUser > mostUses)
                            {
                                mostUses = ordersByAffForThisUser;
                                mostUsedAffiliateId = group.Key;
                                affiliateIdForOrder = group.Key;
                            }


                            if (mostUses >= 2 * numberOfUsesOfMostRecentAffiliate)
                            {
                                affiliateIdForOrder = mostUsedAffiliateId;
                            }
                        }

                        if (mostUses >= 2 * numberOfUsesOfMostRecentAffiliate)
                        {
                            _logger.Info("FindAffForSession by mostUses: " + idUser + " awarded: " +
                                         mostUsedAffiliateId);
                            affiliateIdForOrder = mostUsedAffiliateId;
                        }
                        var aff = ((TTSWebinarsContext)db).Orders
                            .Where(o => o.idAffiliate == affiliateIdForOrder)
                            .OrderByDescending(o => o.OrderDate)
                            //.Include(u => u.WebUser)
                            .Select(o => o.Affiliate).FirstOrDefault();
                        _logger.Info("FindAffForSession returns: " + aff.ttsDomain 
                            + " for: " + FindByIdLoaded(idUser).email);
                        return aff;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.FatalException("FindAffForSession: ", e);
            }
            return null;
        }

        public Presenter GetPresenterById(int userIdUser)
        {
            var presenter = ((TTSWebinarsContext)db).Presenters
                .Where(p => p.idUser == userIdUser).Include(u => u.WebUser).FirstOrDefault();
            return presenter;
        }

        public int? GetUserIdByFirstNameLastName(string fullName)
        {
            var firstName = fullName.Split(' ')[0];
            var lastName = fullName.Split(' ')[1];
            var userId = ((TTSWebinarsContext)db).WebUsers
                .Where(p => p.FirstName == firstName && p.LastName == lastName).Select(p => p.idUser).FirstOrDefault();
            return userId;
        }

        public IList<WebUser> GetWspUsers(int idDiscount)
        {
            return ((TTSWebinarsContext)db).WebUsers
                .Where(p => p.idSubscriptionDiscount == idDiscount).ToList();

        }

        public int? GetWebUserIdByEmail(string email)
        {
            return items.Where(w => w.email == email).Select(w => w.idUser).SingleOrDefault();
        }

        public string GetWebUserFullname(string email)
        {
            return items.Where(w => w.email == email).Select(w => w.FirstName + " " + w.LastName).SingleOrDefault();
        }

        public IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate)
        {

            return ((TTSWebinarsContext)db).Orders.Include(o => o.WebUser)
                .Where(o => o.idAffiliate == idAffiliate &&
                    o.WebUser.LastName.ToLower().Contains(lastName))
                .Select(o => o.WebUser)
                .OrderBy(webUser => webUser.LastName)
                .ThenBy(webUser => webUser.FirstName);
        }

        public WebUser GetWebUserByEmailLoadedWithOrdersData(string email)
        {
            return items
                .Include(wu => wu.Addresses)
                .Include(wu => wu.Institution)
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.Discount)))
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.AdditionalLocation)))
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.RegistrationType)))
                .SingleOrDefault(wu => wu.email == email
                );
        }

        public void UpdateAddresses(Address address)
        {
            var entry = db.Entry(address);

            if (entry.State == EntityState.Detached)
            {
                ((TTSWebinarsContext)db).Addresses.Attach(address);
                entry.State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    var errorMsg = new StringBuilder();

                    errorMsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errorMsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    _logger.Error("From catch block of UpdateAddresses " + errorMsg);
                }
                throw;
            }
        }

        public IEnumerable<WebUser> GetWebusersForWebinarWithRegtypes(int idWebinar, IEnumerable<int> regTypeIds)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => regTypeIds.Contains(or.idRegType))
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }
        public IEnumerable<WebUser> GetWebusersForRecordingPostedNotifications(int idWebinar)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowRecordingNotifications == "Yes")
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }
        public IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowLiveNotifications == "Yes")
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }

        public void Update(WebUser webUser)
        {
            CheckDisposed();

            var entry = db.Entry(webUser);
            if (entry.State == EntityState.Detached)
            {
                items.Attach(webUser);
                entry.State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        public bool WebUserExists(int idUser)
        {
            return items.Any(w => w.idUser == idUser);
        }

        public DbContext DbContext
        {
            get { return db; }
        }

        public WebUser GetWebUserByIdLoadedWithAddressesAndInstitution(int idUser)
        {
            return items.Include(w => w.Addresses)
                .Include(w => w.Institution)
                .SingleOrDefault(w => w.idUser == idUser);
        }

        public void SetUserStatusToUnChanged(WebUser user)
        {
            db.Entry(user).State = EntityState.Unchanged;
        }

        public WebUser BuildPlaceHolderUser(string orderEmail)
        {
            var starterUser = GetWebUserByEmailDomain(orderEmail);

            WebUser newUser = new WebUser
            {
                idUser = FindHighestUserId(),
                email = orderEmail,
                Addresses = new List<Address>(),
                OptedOutOfUpgradePrompt = false,
                generalComments = "AutoGenerated Account. Please update."
            };
            if (starterUser != null)
            {
                newUser.generalComments = "Account seeded by idUser: " + starterUser.idUser;
                newUser.idUserInstitution = starterUser.idUserInstitution;

                Address addBilling = new Address { AddressType = "Billing" };
                Address addShipping = new Address { AddressType = "Shipping" };
                var bill = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Billing");
                addBilling.WebUser = newUser;
                addBilling.City = bill.City;
                addBilling.Country = bill.Country;
                addBilling.Name = bill.Name;
                addBilling.Phone = bill.Phone;
                addBilling.State = bill.State;
                addBilling.StreetAddress = bill.StreetAddress;
                addBilling.StreetAddress2 = bill.StreetAddress2;
                addBilling.Zip = bill.Zip;

                var ship = starterUser.Addresses.SingleOrDefault(a => a.AddressType == "Shipping");
                addShipping.WebUser = newUser;
                addShipping.City = ship.City;
                addShipping.Country = ship.Country;
                addShipping.Name = ship.Name;
                addShipping.Phone = ship.Phone;
                addShipping.State = ship.State;
                addShipping.StreetAddress = ship.StreetAddress;
                addShipping.StreetAddress2 = ship.StreetAddress2;
                addShipping.Zip = ship.Zip;


                newUser.Addresses.Add(addBilling);
                newUser.Addresses.Add(addShipping);

                newUser.timeZone = USTimeZone.Central;
                newUser.AcctStatus = "Seeded";
                newUser.DateCreated = DateTime.Now;
                newUser.FirstName = "tempFirst";
                newUser.LastName = "tempLast";
                newUser.UserType = UserType.Customer;

                Update(newUser);

                return newUser;
            }
            else
            {

                newUser.generalComments = "Account seeded with blank values.";
                newUser.idUserInstitution = 16263;
                Address addBilling = new Address { AddressType = "Billing" };
                Address addShipping = new Address { AddressType = "Shipping" };
                addBilling.WebUser = newUser;
                addBilling.City = "";
                addBilling.Country = "";
                addBilling.Name = "";
                addBilling.Phone = "";
                addBilling.State = "";
                addBilling.StreetAddress = "";
                addBilling.StreetAddress2 = "";
                addBilling.Zip = "";

                addShipping.City = "";
                addShipping.Country = "";
                addShipping.Name = "";
                addShipping.Phone = "";
                addShipping.State = "";
                addShipping.StreetAddress = "";
                addShipping.StreetAddress2 = "";
                addShipping.Zip = "";


                newUser.Addresses.Add(addBilling);
                newUser.Addresses.Add(addShipping);

                newUser.timeZone = USTimeZone.Central;
                newUser.AcctStatus = "Seeded";
                newUser.DateCreated = DateTime.Now;
                newUser.FirstName = "tempFirst";
                newUser.LastName = "tempLast";
                newUser.UserType = UserType.Customer;

                Update(newUser);

                return newUser;
            }
        }


    }
}
