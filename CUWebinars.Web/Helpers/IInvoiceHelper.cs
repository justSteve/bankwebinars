using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using GemBox.Document;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CUWebinars.Web.Helpers
{
    public interface IInvoiceHelper
    {
        JProperty OrderIsCanceled(Order modelOrder, Order order);
        
    }
}