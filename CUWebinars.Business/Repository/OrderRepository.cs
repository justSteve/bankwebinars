using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class OrderRepository : TTSWebinarsRepository<TTSWebinarsContext, Order>, IOrderRepository
    {
        public Order CreateOrder()
        {
            var newOrder = items.Create();
            newOrder.OrderDate = DateTime.Now;
            Add(newOrder);
            db.SaveChanges();
            return newOrder;
        }

        public OrderRow CreateOrderRow(Webinar webinar, Order order, string alternateEmail, int registrationType)
        {
            var strongTypedContext = (TTSWebinarsContext) db;
            var entry = db.Entry(webinar);

            if (entry.State != EntityState.Detached)
                throw new Exception("Webinar must be detached from its original DbContext");

            strongTypedContext.Webinars.Attach(webinar);
            db.Entry(webinar).State = EntityState.Modified;

            var newOrderRow = strongTypedContext.OrderRows.Create();
            newOrderRow.Order = order;
            newOrderRow.Webinar = webinar;
            newOrderRow.RegistrationType = registrationType;
            newOrderRow.AlternateEmail = alternateEmail;
            strongTypedContext.OrderRows.Add(newOrderRow);
            db.SaveChanges();
            return newOrderRow;
        }

        public OrderRowOption CreateOrderRowOption(
            OrderRow orderRow, 
            Option option, 
            string optionDescription, 
            decimal price, 
            string alternateEmail, 
            int additionalLocationsCount,
            string [] additionalLocationsEmails)
        {
            var strongTypedContext = (TTSWebinarsContext) db;
            var entry = db.Entry(option);

            if (entry.State != EntityState.Detached)
                throw new Exception("Webinar must be detached from its original DbContext");

            strongTypedContext.Options.Attach(option);
            db.Entry(option).State = EntityState.Modified;

            var newOrderRowOption = strongTypedContext.OrderRowOptions.Create();
            newOrderRowOption.Option = option;
            newOrderRowOption.idOption = option.idOption;
            newOrderRowOption.idOrderRow = orderRow.idOrderRow;
            newOrderRowOption.OrderRow = orderRow;
            newOrderRowOption.OptionDescription = optionDescription;
            newOrderRowOption.OptionPrice = price;
            newOrderRowOption.Type = "Additional Location"; // TODO: check this with Stephen.
            strongTypedContext.OrderRowOptions.Add(newOrderRowOption);

            try
            {
                db.SaveChanges();
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
            
            return newOrderRowOption;
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
            if (db.SaveChanges() > 0)
            {
                //db.Entry(order).Reference(o => o.WebUser).Load();
                return order;
            }

            throw new Exception(""); // TODO: come up with meaningful exception and msg
        }

        public virtual IDictionary<Option, Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            var optionAndOrder = new Dictionary<Option, Order>();

            items.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Scheduled
                                          &&
                                          (o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Submitted ||
                                           o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Paid)
                )
                .ToList()
                .ForEach(o =>
                {
                    var row = o.OrderRows.Single();
                    var paramWebinarID = new SqlParameter("idOption", SqlDbType.Int) { Value = (int)row.RegistrationType };
                    var registrationType = ((TTSWebinarsContext)db).Options.SqlQuery("dbo.GetRegistrationType @idOption", paramWebinarID).Single();

                    optionAndOrder.Add(registrationType, o);
                }
               );

            return optionAndOrder;
        }

        public virtual IList<Order> Test(int idUser)
        {
            var optionAndOrder = new Dictionary<Option, Order>();

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
                                          (o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Submitted ||
                                           o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Paid)
                )
                .ToList();
        }

        public virtual IList<Order> SelectOrdersWithArchivedWebinars(int idUser)
        {
            return items.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Archived
                                          &&
                                          (o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Submitted ||
                                           o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Paid)
                )
                .ToList();

        }
    }
}
