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
        readonly DisconnectedPropertyChangeHelper _disconnectedPropertyChangeHelper;
        public OrderRepository()
        {
            _disconnectedPropertyChangeHelper = new DisconnectedPropertyChangeHelper(db);
        }

        public Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {
            var newOrder = items.Create();
            newOrder.OrderDate = DateTime.Now;

            newOrder.Affiliate = affiliate;
            newOrder.WebUser = webUser;

            //  Cannot load this from the database, as the Addresses collection of
            //  WebUser cannot be loaded. Get the WebUser populated via the WebUserRepository.
            //newOrder.WebUser = webUser;
            //newOrder.OrderRows.
            newOrder.OrderRows = new List<OrderRow>();
            newOrder.OrderRows.Add(orderRow);
            
            //Add(newOrder);
            _disconnectedPropertyChangeHelper.ApplyChanges(newOrder);
            
            //db.SaveChanges();

            //  Best to load these from the database
            //db.Entry(newOrder).Reference(no => no.Affiliate).Load();
            //db.Entry(newOrder).Reference(no => no.WebUser).Load();
            //_disconnectedPropertyChangeHelper.MaterializeObject(newOrder);
            //_disconnectedPropertyChangeHelper.MaterializeObject(newOrder.OrderRows.First());

            return newOrder;
        }

        public OrderRow CreateOrderRow(Webinar webinar, AdditionalLocation additionalLocation, RegType registrationType)
        {
            var strongTypedContext = (TTSWebinarsContext) db;
            //var entry = strongTypedContext.Entry(webinar);

            //if (entry.State != EntityState.Detached)
            //    throw new Exception("Webinar cannot be attached to this context yet. It has to have been retrieved and previously detached.");

            //strongTypedContext.Webinars.Attach(webinar);
            //entry.State = EntityState.Modified;
            
            var newOrderRow = strongTypedContext.OrderRows.Create();

            if (additionalLocation != null)
                newOrderRow.AdditionalLocation.Add(additionalLocation);

            newOrderRow.Webinar = webinar;
            newOrderRow.RegistrationType = registrationType;
            newOrderRow.RowStatus = OrderRowStatus.Active;
            
            //strongTypedContext.OrderRows.Add(newOrderRow);

            try
            {
                //db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

            return newOrderRow;
        }

        public AdditionalLocation CreateAdditionalLocation(decimal price, string fullName, string email)

        {
            var strongTypedContext = (TTSWebinarsContext) db;
            //var entry = db.Entry(RegType);

            //if (entry.State != EntityState.Detached)
            //    throw new Exception("Webinar must be detached from its original DbContext");

            //strongTypedContext.Options.Attach(RegType);
            //db.Entry(RegType).State = EntityState.Modified;

            //var AdditionalLocation = new List<AdditionalEmails>(AdditionalLocationEmails.Length);
            
            //AdditionalLocation.AddRange(AdditionalLocationEmails.Select(email => new AdditionalEmails
            //{
            //    Email = email
            //}));


            var additionalLocation = strongTypedContext.AdditionalLocation.Create();
            
            additionalLocation.Price = price;
            additionalLocation.Email= email;
            additionalLocation.FullName = fullName;
            
            strongTypedContext.AdditionalLocation.Add(additionalLocation);

            try
            {
                //db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            
            return additionalLocation;
        }

        public Order FindOrderByIdWithOrderRows(int id)
        {
            var item = items
                //.Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                .Where(o => o.idOrder == id);
            return item.FirstOrDefault();
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
            return items.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Recorded
                                          &&
                                          (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid
                                           )
                )
                .ToList();
        }
        public virtual IList<Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            return items.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Scheduled
                                          &&
                                          (o.OrderStatus == OrderStatus.Submitted ||
                                           o.OrderStatus == OrderStatus.Paid)
                )
                .ToList();
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
