using System.Security.Cryptography;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Validation.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class OrderRepository : TTSWebinarsRepository<TTSWebinarsContext, Order>, IOrderRepository
    {
        private readonly IValidator<Order> _orderValidator;

        public OrderRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {
            _orderValidator = new CreateOrderValidator(new WebinarRepository((TTSWebinarsContext)db));
        }

        public Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null)
        {

            var newOrder = items.Create();
            newOrder.OrderDate = DateTime.Now;
            newOrder.OrderStatus = OrderStatus.InProcess;
            newOrder.Affiliate = affiliate;
            newOrder.BillingEmail = webUser.email;
            newOrder.idUser = webUser.idUser;
            newOrder.Origin = origin;

            newOrder = AssignWebUserToOrder(webUser, newOrder);

            newOrder.OrderRows = new List<OrderRow>();
            newOrder.OrderRows.Add(orderRow);

            if (webUser.idSubscriptionDiscount != null && webUser.idSubscriptionDiscount > 0)
            {
                orderRow.Discount = GetUserDiscount(webUser.idUser);
            }

            //_orderValidator.ValidateAndThrow(newOrder);
            var validationResult = _orderValidator.Validate(newOrder);

            if (validationResult.IsValid)
            {
                Add(newOrder); // save changes is called in here.

                return newOrder;
            }

            var errors = ValidationHelper.GetMessagesAsXmlElement(validationResult.Errors);
            throw new Exception(errors.ToString());
        }

        public OrderRow CreateOrderRow(Webinar webinar,
            IList<AdditionalLocation> additionalLocations,
            RegType registrationType)
        {
            try
            {
                var strongTypedContext = (TTSWebinarsContext)db;

                var newOrderRow = strongTypedContext.OrderRows.Create();

                if (additionalLocations != null)
                    newOrderRow.AdditionalLocation = additionalLocations;

                newOrderRow.Webinar = webinar;
                newOrderRow.RegistrationType = registrationType;
                newOrderRow.RowStatus = OrderRowStatus.Active;

                // The RowPrice is just the starting point. The full price for an 
                // order is calculated in CalculateOrderCost of the OrderManagementService
                newOrderRow.RowPrice = Convert.ToDecimal(newOrderRow.RegistrationType.Price);

                return newOrderRow;

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }

        public void DeleteOrder(int orderId)
        {
            var orderToDelete = items.Find(orderId);

            Remove(orderToDelete);
        }

        public AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullName)
        {
            try
            {
                var strongTypedContext = (TTSWebinarsContext)db;

                var additionalLocation = strongTypedContext.AdditionalLocation.Create();

                additionalLocation.Price = price;
                additionalLocation.Email = email;
                additionalLocation.FullName = fullName;

                strongTypedContext.AdditionalLocation.Add(additionalLocation);

                return additionalLocation;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);

                    }
                }
                throw;
            }
        }

        public Order FindOrderByIdWithOrderRows(int id)
        {
            var item = items
                .Include(o => o.WebUser)
                .Include(o => o.Affiliate)
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Include(o => o.OrderRows.Select(or => or.Webinar.Presenter.WebUser))
                .Include(o => o.OrderRows.Select(or => or.Webinar.WebinarFiles))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.Discount))
                .Where(o => o.idOrder == id);
            return item.FirstOrDefault();
        }

        public IList<Order> FindOrdersByUserId(int userId)
        {
            var userOrders = items.Include(o => o.OrderRows.Select(or => or.Webinar))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Where(o => o.WebUser.idUser == userId);

            return ReferenceEquals(null, userOrders) ? null : GetLoadedEntitiesForOrder(userOrders);

        }

        public IList<int> FindOrderIdsByPartialId(int userId)
        {
            return items.Where(order => order.idOrder.ToString().Contains(userId.ToString()))
                .Select(order => order.idOrder)
                .ToList();
        }

        public IList<Order> FindOrdersByUserIdWithOrderRows(int userId)
        {
            var userOrders = items.Include(o => o.OrderRows.Select(or => or.Webinar))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Where(o => o.WebUser.idUser == userId);

            return ReferenceEquals(null, userOrders) ? null : GetLoadedEntitiesForOrder(userOrders);

        }

        public IList<Order> GetOrdersForLiveEventNotifications(int idWebinar)
        {
            var orders = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Include(row => row.Webinar)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowLiveNotifications == "Yes")
                .Where(or => or.RowStatus == OrderRowStatus.Active)
                .Select(o => o.Order);

            return GetLoadedEntitiesForOrder(orders);
        }
        public IList<Order> GetInactiveOrdersByWebinar(int idWebinar)
        {
            var orders = ((TTSWebinarsContext)db).OrderRows

                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.Order.OrderStatus != OrderStatus.Paid ||
                    or.Order.OrderStatus != OrderStatus.Billed ||
                    or.Order.OrderStatus != OrderStatus.Submitted
                    )
                .Select(o => o.Order);
            return GetLoadedEntitiesForOrder(orders);
        }

        public IList<Order> GetActiveOrdersByWebinar(int idWebinar)
        {
            var orders = ((TTSWebinarsContext)db).OrderRows

                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.Order.OrderStatus == OrderStatus.Paid ||
                    or.Order.OrderStatus == OrderStatus.Billed ||
                    or.Order.OrderStatus == OrderStatus.Submitted
                    )
                .Select(o => o.Order);
            return GetLoadedEntitiesForOrder(orders);
        }

        public IList<Order> GetOrdersForRecordedEventNotifications(int idWebinar)
        {
            //TODO: Refactor to employ Claims inspection
            var orders = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowRecordingNotifications == "Yes")
                .Select(o => o.Order);
            return GetLoadedEntitiesForOrder(orders);
        }
        public IList<Order> GetOrdersForShippedEventNotifications()
        {
            var orders = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.RegistrationType.ShowShippedNotifications == "Yes")
                .Select(o => o.Order);
            return GetLoadedEntitiesForOrder(orders);
        }

        public OrderRow GetOrderRowById(int idOrderRow)
        {
            return ((TTSWebinarsContext)db).OrderRows.Include(or => or.Webinar)
                .Include(or => or.Order.WebUser.Institution)
                .Include(or => or.Order.WebUser.Presenter)
                .Include(or => or.Order.WebUser.Addresses)
                .Include(or => or.Order.Affiliate)
                .Include(or => or.RegistrationType)
                .Include(or => or.AdditionalLocation)
                .Include(or => or.Discount)
                .SingleOrDefault(or => or.idOrderRow == idOrderRow);
        }

        /// <summary>
        /// Only use this method if the affiliate is already attached to the context.
        /// </summary>
        /// <param name="affiliate"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public Order AssignAffiliate(Affiliate affiliate, Order order)
        {
            order.idAffiliate = affiliate.idUserAff;
            if (db.SaveChanges() > 0)
            {
                db.Entry(order).Reference(o => o.Affiliate).Load();
                return order;
            }

            return order;
        }

        public Order AssignWebUserToOrder(WebUser webUser, Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            order.idUser = webUser.idUser;

            //  
            if (webUser.Addresses == null)
                return order;

            var billingAddress =
                webUser.Addresses.FirstOrDefault(a => a.AddressType == DomainConstants.BillingAddress);
            var shippingAddress =
                webUser.Addresses.FirstOrDefault(a => a.AddressType == DomainConstants.ShippingAddress);

            if (billingAddress != null)
            {
                order.BillingAddress = billingAddress.StreetAddress;
                order.BillingCity = billingAddress.City;
                order.BillingEmail = webUser.email;
                order.FirstName = webUser.FirstName;
                order.LastName = webUser.LastName;
                order.BillingPhone = billingAddress.Phone;
                order.BillingZip = billingAddress.Zip;
                order.BillingState = billingAddress.State;

                if (shippingAddress != null)
                {
                    order.ShippingAddress = shippingAddress.StreetAddress;
                    order.ShippingCity = shippingAddress.City;
                    order.ShippingFirstName = webUser.FirstName;
                    order.ShippingLastName = webUser.LastName;
                    order.ShippingPhone = shippingAddress.Phone;
                    order.ShippingState = shippingAddress.State;
                    order.ShippingZip = shippingAddress.Zip;
                    order.Institution = webUser.Institution.InstitutionName;

                    //if (user.SubscriptionDiscount != null)
                    //{
                    //    foreach (OrderRow row in order.Rows)
                    //    {
                    //        row.DiscountCode = user.SubscriptionDiscount.Code;
                    //    }
                    //}
                    //try
                    //{
                    //    Save(order);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Logger.Instance.LogException(ex);
                    //    Logger.Instance.LogMessage("ERROR: Failed to save user assigned to order: "+ user.Email + " order = " + order.ID);
                    //    throw;
                    //}
                }
            }

            if (order.Origin != null)
            {
                switch (order.Origin)
                {
                    case DomainConstants.OriginMigrated:
                    case DomainConstants.OriginImported:
                        return order;
                }
            }

            db.SaveChanges();

            return order;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public Order SaveOrderChanges(Order order, int? isFromSignup = null)
        {
            var error = db.GetValidationErrors().ToArray();

            if (error.Any())
            {
                foreach (var err in error)
                {
                    order.AdminComments += err.Entry.ToString();
                }

            }

            db.SaveChanges();

            return order;
        }


        public int AccessToPostEventMaterials(int webinar, int user)
        {
            var a = items
                    .Where(o => o.idUser == user
                        && o.OrderRows.FirstOrDefault().idWebinar == webinar
                        && (o.OrderStatus == OrderStatus.Submitted
                            || o.OrderStatus == OrderStatus.Paid
                            || o.OrderStatus == OrderStatus.Billed
                            )
                        && o.OrderRows.FirstOrDefault().RegistrationType.ShowRecordingNotifications == "Yes"
                        )
                    .ToList();

            //TODO: Refactor to employ Claims inspection
            return !items.Any() ? 0 : 1;
        }

        public Discount FindDiscountById(int id)
        {
            return ((TTSWebinarsContext)db).Discounts.SingleOrDefault
                (d => d.idDiscount == id);

        }

        public Discount FindDiscountByCode(string discount)
        {
            return ((TTSWebinarsContext)db).Discounts.SingleOrDefault
                (d => d.DiscountCode == discount);

        }

        public Discount FindDiscountByUser(WebUser currentUser)
        {
            Discount myDiscount = null;
            if (currentUser.idSubscriptionDiscount != null)
            {
                myDiscount = FindDiscountById(currentUser.idSubscriptionDiscount.Value);
            }
            return myDiscount;
        }

        public int GetNumberOfOrdersPerWebinar(int id)
        {

            return items.Count(o => o.OrderRows.FirstOrDefault().RowStatus == OrderRowStatus.Active &&  (o.OrderStatus == OrderStatus.Submitted
                || o.OrderStatus == OrderStatus.Paid
                || o.OrderStatus == OrderStatus.Billed
                ));

        }

        //public IList<AdditionalLocation> GetAdditionalLocations(int idOrder)
        //{
        //    var additionalLocations = items
        //        .Include(o => o.OrderRows.Select(a => a.AdditionalLocation )
        //        .Where(o => o.idOrder == idOrder));
        //    return null;
        //}

        public virtual IList<Order> SelectOrdersWithRecordedWebinars(int idUser)
        {
            var orders = items
                .Include(o => o.OrderRows.Select(w => w.RegistrationType))
                .Include(o => o.OrderRows.Select(w => w.Webinar))
                .Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Recorded
                && (o.OrderStatus == OrderStatus.Submitted
                || o.OrderStatus == OrderStatus.Paid
                || o.OrderStatus == OrderStatus.Billed
                )
                    );
            return GetLoadedEntitiesForOrder(orders);

        }

        public virtual IList<Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            var orders = items
                .Include(o => o.OrderRows.Select(w => w.RegistrationType))
                .Include(o => o.OrderRows.Select(w => w.Webinar))
                .Where(o => o.idUser == idUser
                                          && (o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Scheduled
                                                || o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.InProgress
                                                || o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Active)
                                          &&
                                          (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid ||
                                           o.OrderStatus == OrderStatus.Billed
                                           )
                );
            return GetLoadedEntitiesForOrder(orders);
        }

        public virtual IList<Order> SelectOrdersWithArchivedWebinars(int idUser)
        {
            var orders = items
                .Include(o => o.OrderRows.Select(w => w.RegistrationType))
                .Include(o => o.OrderRows.Select(w => w.Webinar))
                .Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Archived
                                          && (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid ||
                                           o.OrderStatus == OrderStatus.Billed
                                           )
                );
            return GetLoadedEntitiesForOrder(orders);
        }

        public virtual IList<Order> GetLoadedEntitiesForOrder(IQueryable<Order> orders)
        {
            return orders.Include(o => o.WebUser)
                .Include(o => o.Affiliate)
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Include(o => o.OrderRows.Select(or => or.Webinar.Presenter.WebUser))
                .Include(o => o.OrderRows.Select(or => or.Webinar.WebinarFiles))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.Discount))
                .ToList();
        }
        public virtual Discount GetUserDiscount(int idUser)
        {
            var usercode = ((TTSWebinarsContext)db).WebUsers.Where(u => u.idUser == idUser).Select(u => u.idSubscriptionDiscount).Single();

            var discount = ((TTSWebinarsContext)db).Discounts.Where(d => d.idDiscount == usercode).Single();
            return discount;

        }
        public virtual bool IsDiscountCodeValid(string discountCode)
        {
            return true;
        }

    }
}
