using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Validation.Order;
using FluentValidation;

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

        public Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {
            var newOrder = items.Create();
            newOrder.OrderDate = DateTime.Now;
            newOrder.OrderStatus = OrderStatus.InProcess;

            newOrder.idAffiliate = affiliate.idUserAff;
            newOrder.idUser = webUser.idUser;

            newOrder.OrderRows = new List<OrderRow>();
            newOrder.OrderRows.Add(orderRow);

            //_orderValidator.ValidateAndThrow(newOrder);
            var validationResult = _orderValidator.Validate(newOrder);

            if (!validationResult.IsValid)
            {
                Add(newOrder); // save changes is called in here.

                //  Best to load these from the database
                //db.Entry(newOrder).Reference(no => no.Affiliate).Load();
                //db.Entry(newOrder).Reference(no => no.WebUser).Load();
                //db.ChangeTracker.DetectChanges();

                return newOrder;
            }
            else
            {
                throw new Exception("");
                //  and do whatever else we come up with here
            }
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
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Include(o => o.OrderRows.Select(or => or.Webinar.Presenter.WebUser))
                .Include(o => o.OrderRows.Select(or => or.Webinar.WebinarFiles))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.Discount))
                .Where(o => o.idOrder == id);
            return item.FirstOrDefault();
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

        public IList<Order> GetOrdersForRecordedEventNotifications(int idWebinar)
        {
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
            return ((TTSWebinarsContext) db).OrderRows.Include(or => or.Webinar)
                .Include(or => or.Order.WebUser.Institution)
                .Include(or => or.Order.WebUser.Presenter)
                .Include(or => or.Order.WebUser.Addresses)
                .Include(or => or.Order.Affiliate)
                .Include(or => or.RegistrationType)
                .Include(or => or.AdditionalLocation)
                .SingleOrDefault(or => or.idOrderRow == idOrderRow);
        }

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
            order.idUser = webUser.idUser;

            //if (db.SaveChanges() > 0)
            //{
            //    //db.Entry(order).Reference(o => o.WebUser).Load();
            //    return order;
            //}

            return order;
        }

        public Order SaveOrderChanges(Order order)
        {
            
            var error = db.GetValidationErrors();
            //is this statement waiting for additional code to
            // trap any remaining errors?
            
            // is the following 
            if (error.Any())
            {
                foreach (var err in error)
                {
                    order.AdminComments += err.Entry.ToString();
                }
                
            }
            else
            {
                order.OrderStatus = OrderStatus.Submitted;
            }

            db.SaveChanges();

            return order;
        }


        public int CheckUserForRecordingAccess(int webinar, int user)
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
            return !items.Any() ? 0 : 1;
        }

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
    }
}
