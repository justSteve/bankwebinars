using System.Collections.Generic;
using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Models;
using System.Data.SqlClient;
using System.Data;

namespace CUWebinars.Web.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private TTSWebinarsContext _ctx;

        public OrderRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }

        public virtual IDictionary<Option, Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            var optionAndOrder = new Dictionary<Option, Order>();

            _ctx.Orders.Where(o => o.idUser == idUser
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
                            var registrationType = _ctx.Options.SqlQuery("dbo.GetRegistrationType @idOption", paramWebinarID).Single();

                            optionAndOrder.Add(registrationType, o);
                        }
               );

            return optionAndOrder;
        }

        public virtual IList<Order> SelectOrdersWithRecordedWebinars(int idUser)
        {
            return _ctx.Orders.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Recorded
                                          &&
                                          (o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Submitted ||
                                           o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Paid)
                )
                .ToList();
        }

        public virtual IList<Order> SelectOrdersWithArchivedWebinars(int idUser)
        {
            return _ctx.Orders.Where(o => o.idUser == idUser
                                          && o.OrderRows.FirstOrDefault().Webinar.Status == WebinarStatus.Archived
                                          &&
                                          (o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Submitted ||
                                           o.OrderRows.FirstOrDefault().Status == OrderRowStatus.Paid)
                )
                .ToList();

        }
    }

}