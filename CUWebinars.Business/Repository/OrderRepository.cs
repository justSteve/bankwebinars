using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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

        public Order AssignAffiliate(Affiliate affiliate, Order order)
        {
            order.idAffiliate = affiliate.idUserAff;

            //if (db.SaveChanges() > 0)
            //{
            //    //db.Entry(order).Reference(o => o.Affiliate).Load();
            //    return order;
            //}

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

        public Order SaveOrder(Order order)
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
