using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CUWebinars.Web.Helpers
{
    public class InvoiceHelper : IInvoiceHelper
    {
        private readonly IStateService _stateService;
        private readonly ILogger _logger;

        private readonly HttpRequestBase _request;
        public readonly TtsConfigHelper _globalConfig;

        public InvoiceHelper(HttpRequestBase request, IStateService stateService, ILogger logger)
        {
            _request = request;
            _stateService = stateService;
            _logger = logger;
            _globalConfig = new TtsConfigHelper();
        }

        public JProperty OrderIsCanceled(Order modelOrder, Order order)
        {

            var dataOperationsV3 = new DataOperations(TtsConfig.DefaultConnectionString);

            string preSaveValues = dataOperationsV3.GetPreSaveValues(order.idOrder);

            _logger.Warn("Invoiced Order is canceled: " + order.idOrder);

            var toJson = JObject.Parse(order.InvoiceDetail);
            var thisInvoice =
                toJson.Properties().FirstOrDefault(p => p.Name.StartsWith("OrderIsInvoiced"));

            if (thisInvoice == null)
                return null;

            var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            //
            row.Royalty = 0;

            var regTypeShortened =
                preSaveValues.Split(',')[4].Replace(" Package", "")
                    .Replace("Live Plus Six", "Live+6")
                    
                    .Replace(" Recording Only", "")
                    .Replace(" Plus Five", "+5");
            StringBuilder sb = new StringBuilder();

            sb.Append(order.idOrder + " was first invoiced on " + thisInvoice.First()["InvoiceId"] +
                      " as type '" + regTypeShortened + "' for $" +
                      preSaveValues.Split(',')[0].ToString().Replace(".0000", "").Replace(".00", "") +
                      ") ");
            sb.Append(" but was canceled " +
                      TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");

            var adustmentAmount = 0 - (decimal)thisInvoice.First()["AmountOfRoyalty"];

            //set default direction
            var adjustmentDirection = "Royalty is decreased";

            sb.Append(adjustmentDirection + " by " +
                      adustmentAmount.ToString("C").Replace(".00", ""));

            row.RowPrice = 0;
            var newJson4Invoice = new JProperty(
                "ChangedOrderNeedsNewInvoice",
                new JObject(
                    new JProperty("OriginalInvoice", thisInvoice.First()["InvoiceId"].ToString()),
                    new JProperty("OriginalDateOfInvoice",
                        thisInvoice.First()["DateOfInvoice"].ToString()),
                    new JProperty("OriginalTotal", thisInvoice.First()["AmountOfOrder"].ToString()),
                    new JProperty("OriginalPercentPaid",
                        thisInvoice.First()["PercentPaid"].ToString()),
                    new JProperty("OriginalRoyaltyPaid",
                        thisInvoice.First()["AmountOfRoyalty"].ToString()),
                    new JProperty("OriginalAffiliate", thisInvoice.First()["Affiliate"].ToString()),
                    new JProperty(adjustmentDirection, adustmentAmount),
                    new JProperty("DateOfChange",
                        TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                    new JProperty("Message", sb.ToString()),
                    new JProperty("UnDo", order.InvoiceDetail)
                ));

            return newJson4Invoice;
        }
    }
}