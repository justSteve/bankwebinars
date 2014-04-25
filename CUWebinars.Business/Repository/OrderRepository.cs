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

namespace CUWebinars.Business.Repository
{
    public class OrderRepository : TTSWebinarsRepository<TTSWebinarsContext, Order>, IOrderRepository
    {
        public OrderRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {

        }

        public Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {
            var newOrder = items.Create();
            newOrder.OrderDate = DateTime.Now;

            newOrder.idAffiliate = affiliate.idUserAff;
            newOrder.idUser = webUser.idUser;

            newOrder.OrderRows = new List<OrderRow>();
            newOrder.OrderRows.Add(orderRow);

            Add(newOrder);

            db.SaveChanges();

            //  Best to load these from the database
            db.Entry(newOrder).Reference(no => no.Affiliate).Load();
            db.Entry(newOrder).Reference(no => no.WebUser).Load();

            return newOrder;
        }

        public OrderRow CreateOrderRow(Webinar webinar, 
            IList<AdditionalLocation> additionalLocations, 
            RegType registrationType)
        {
            try
            {
                var strongTypedContext = (TTSWebinarsContext) db;

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
                var strongTypedContext = (TTSWebinarsContext) db;

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
                //.Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Where(o => o.idOrder == id);
            return item.FirstOrDefault();
        }

        public IList<Order> FindOrdersByUserIdWithOrderRows(int userId)
        {
            var userOrders = items.Include(o => o.OrderRows.Select(or => or.Webinar))
                .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Where(o => o.WebUser.idUser == userId);

            return ReferenceEquals(null, userOrders) ? null : userOrders.ToList();

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
            Debug.WriteLine("#######################Call to SaveOrderChanges");

            //_disconnectedPropertyChangeHelper.ApplyChanges(order);

            db.SaveChanges();

            return order;
        }

        //public virtual IDictionary<RegType, Order> SelectOrdersWithScheduledWebinars(int idUser)
        //{
        //    var optionAndOrder = new Dictionary<RegType, Order>();

        //    items.Where(o => o.idUser == idUser
        //                                  && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Scheduled
        //                                  &&
        //                                  (o.OrderStatus == OrderStatus.Submitted ||
        //                                   o.OrderStatus == OrderStatus.Paid)
        //        )
        //        .ToList()
        //        .ForEach(o =>
        //        {
        //            var row = o.OrderRows.Single();
        //            var paramWebinarID = new SqlParameter("idRegType", SqlDbType.Int) { Value = (int)row.RegistrationType };
        //            var registrationType = ((TTSWebinarsContext)db).Options.SqlQuery("dbo.GetRegistrationType @idRegType", paramWebinarID).Single();

        //            optionAndOrder.Add(registrationType, o);
        //        }
        //       );

        //    return optionAndOrder;
        //}

        public virtual IList<Order> Test(int idUser)
        {
            var optionAndOrder = new Dictionary<RegType, Order>();

            var list = items.Include("OrderRows")
                        .Where(o => o.idUser == idUser)
                        .ToList();

            return list;
        }

        public virtual IList<Order> SelectOrdersWithRecordedWebinars(int idUser)
        {
            var a = items
                .Include(o => o.OrderRows.Select(w => w.RegistrationType))
                .Include(o => o.OrderRows.Select(w => w.Webinar))
                .Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Recorded
                //&& (o.OrderStatus == OrderStatus.Submitted 
                //|| o.OrderStatus == OrderStatus.Paid)
                    )
                .ToList();
            return a;

        }

        public virtual IList<Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            var i  = items
                .Include(o => o.OrderRows.Select(w => w.RegistrationType))
                .Include(o => o.OrderRows.Select(w => w.Webinar))
                .Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Scheduled
                                          &&
                                          (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid)
                )
                .ToList();
            return i;
        }

        public virtual IList<Order> SelectOrdersWithArchivedWebinars(int idUser)
        {
            return items.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Archived
                                          &&
                                          (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid)
                )
                .ToList();

        }
    }
}
