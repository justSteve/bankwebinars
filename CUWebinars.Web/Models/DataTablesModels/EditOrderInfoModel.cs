using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CUWebinars.Web.Models.DataTablesModels
{
    public class EditOrderInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturnUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditOrder_Compact")]
        public EditOrderModel EditFields { get; set; }
    }
}