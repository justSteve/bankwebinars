using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CUWebinars.Web.Models.DataTablesModels
{
    public class EditResendsInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturnUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("ResendInfo_Compact")]
        public EditResendsModel EditFields { get; set; }
        
    }
}